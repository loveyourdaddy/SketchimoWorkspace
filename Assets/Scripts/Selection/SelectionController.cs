using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Sketchimo.Models;

public enum PlaybarState { Play, Pause, Stop } 

[System.Serializable]
public struct UI
{
	public GameObject buttonsPanel;
	public Slider playBar;

	public AnimatorController animcontroller;
	[HideInInspector]
	public List<GameObject> poolingObjects;
	[HideInInspector]
	public string currentPlayAnim;
	public GameObject[] baseButton;
}

[System.Serializable]
public struct CharButtonInfo
{
	public GameObject character;
	public Sprite image;
	public float size;

	public CharButtonInfo(GameObject c, Sprite s, float sz = 1)
	{
		character = c;
		image = s;
		size = sz;
	}
}

[System.Serializable]
public struct MotionButtonInfo
{
	public MotionData motion;
	public MotionData mixamo;
	public Sprite image;

	public MotionButtonInfo(MotionData m, MotionData mi, Sprite s)
	{
		motion = m;
		mixamo = mi;
		image = s;
	}
}

public class SelectionController : MonoBehaviour
{
	[Header("UI")]
	public GameObject overview;
	public UI baseUI;
	public List<CharButtonInfo> characters;
	public List<MotionButtonInfo> motions;
	public MotionButtonInfo tPose;
	
	[HideInInspector]
	public static MotionData currentMotion;
	public static GameObject currentModel;
	public static Texture2D currentTexture;
	public static float characterScale = 1f;

	private int currentPanelNum;
	private PlaybarState playbarState = PlaybarState.Play;
	private bool isStop = false;

	[HideInInspector]
	public int frame = 0;
	private GameObject[] _bones;
	private GameObject _mesh;
	private float timerValue = 0;

	private readonly Dictionary<GameObject, int> _indexOfBone = new Dictionary<GameObject, int>();
	private List<GameObject>[] objectPoolList = new List<GameObject>[] { new List<GameObject>(), new List<GameObject>() };
	private void Start()
	{
		//LoadMotionData();
		LoadSources();
		SwitchPanel(0);

		if (Sketchimo.Controllers.SceneOverview.Instance == null)
			Instantiate(overview);
	}

	public void Update()
	{
		if (_mesh != null && Timer() && currentMotion != null && !isStop)
		{
			switch(playbarState)
			{
				case PlaybarState.Play:
					frame += 1;
					frame = frame >= currentMotion.totalFrame ? 0 : frame;
					SetPose(currentMotion.data[Mathf.Clamp(frame, 0, currentMotion.totalFrame - 1)]);
					break;

				case PlaybarState.Pause:
					SetPose(currentMotion.data[Mathf.Clamp(frame, 0, currentMotion.totalFrame - 1)]);
					break;

				default:
					break;
			}

			baseUI.playBar.SetValueWithoutNotify((float)frame / currentMotion.totalFrame);
		}
	}

	public bool Timer()
	{
		if (currentMotion == null)
			return false;

		timerValue += Time.deltaTime;

		if (timerValue >= 1.0f / currentMotion.fps)
		{
			timerValue = 0f;
			return true;
		}

		return false;
	}

	public void LoadMotionData()
	{
		string[] tempMotions = AssetDatabase.FindAssets("", new string[] { "Assets/output" });
		for (int i = 0; i < tempMotions.Length; i++)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(tempMotions[i]);
			MotionData tempMotion = (MotionData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MotionData));
			motions.Add(new MotionButtonInfo(tempMotion, tempMotion, null));
		}
	}

	public void SetPose(PoseData pose)
	{
		_bones[0].transform.position = pose.joints[0].position * characterScale;
		_bones[0].transform.rotation = pose.joints[0].rotation;

		for (var i = 1; i < _bones.Length; i++)
		{
			var bone = _bones[i];
			var newRoot = pose.joints[0];
			var newBone = pose.joints[i];

			bone.transform.position = (newRoot.position + newRoot.rotation * newBone.position) * characterScale;
			bone.transform.rotation = newRoot.rotation * newBone.rotation;
		}
	}

	public void LoadMeshData(GameObject mesh)
	{
		_mesh = mesh;
		_bones = _mesh.GetComponentsInChildren<Transform>().Select(p => p.gameObject).ToArray();

		_indexOfBone.Clear();
		for (var i = 0; i < _bones.Length; i++)
			_indexOfBone[_bones[i]] = i;
	}


	public void SwitchPanel(int panelNum)
	{
		foreach(List<GameObject> list in objectPoolList)
		{
			foreach (GameObject obj in list)
			{
				obj.SetActive(false);
			}
		}

		foreach(GameObject i in objectPoolList[panelNum])
		{
			i.SetActive(true);
		}

		currentPanelNum = panelNum;
	}

	private void LoadSources()
	{
		//update the characters
		foreach (CharButtonInfo o in characters)
		{
			GameObject instance_Button = Instantiate(baseUI.baseButton[0], baseUI.buttonsPanel.transform);
			SelectionInfo_Character instance_Info = instance_Button.GetComponent<SelectionInfo_Character>();

			instance_Info.Init(o);
			objectPoolList[0].Add(instance_Button);
			instance_Button.SetActive(false);
		}

		//update the animations
		for(int i = 0; i < motions.Count; i++)
		{
			GameObject instance_Button = Instantiate(baseUI.baseButton[1], baseUI.buttonsPanel.transform);
			SelectionInfo_Animation instance_Info = instance_Button.GetComponent<SelectionInfo_Animation>();

			instance_Info.Init(motions[i]); 
			objectPoolList[1].Add(instance_Button);
			instance_Button.SetActive(false);
		}
	}

	public void SearchField(InputField text)
	{
		foreach(GameObject i in objectPoolList[currentPanelNum])
		{
			i.SetActive(i.name.Replace(".motion", "").ToLower().Contains(text.text.ToLower()));
		}
	}

	public void Stop()
	{
		frame = 0;
		baseUI.playBar.value = 0;
		SetPose(currentMotion.data[0]);
		playbarState = PlaybarState.Stop;
	}

	public void Play()
	{
		playbarState = PlaybarState.Play;
	}

	public void Pause()
	{
		playbarState = PlaybarState.Pause;
	}

	public void PlaybarDown()
	{
		isStop = true;
	}

	public void PlaybarUp()
	{
		isStop = false;
	}
	public void SelectionComplete()
	{
		if(currentMotion != null && currentModel != null)
			SceneManager.LoadScene("EditScene");
	}

	public void ChangeValue()
	{
		if (currentMotion != null)
		{
			frame = (int)(baseUI.playBar.value * currentMotion.totalFrame);
			SetPose(currentMotion.data[Mathf.Clamp(frame, 0, currentMotion.totalFrame - 1)]);
		}
	}

}
