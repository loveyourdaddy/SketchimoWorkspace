using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Sketchimo.Models
{
public class SelectionInfo_Character : SelectionInfo
{
    public GameObject model;
	public float size;

	public override void Awake()
	{
		base.Awake();
		controller = FindObjectOfType<SelectionController>();
	}

	public void Init(CharButtonInfo model_)
	{
		text.text = model_.character.name;
		model = model_.character;
		gameObject.name = model_.character.name;
		titleImage.sprite = model_.image;
		size = model_.size;
	}

	public override void ClickButton()
	{
		SelectionController.currentTexture = null;
		SelectionController.characterScale = size;

		foreach (GameObject i in controller.baseUI.poolingObjects)
		{
			i.SetActive(false);
		}

		if (poolingObject == null)
		{
			poolingObject = Instantiate(model, Vector3.zero, Quaternion.identity);
			poolingObject.transform.localScale = Vector3.one * SelectionController.characterScale;
			controller.baseUI.poolingObjects.Add(poolingObject);
			if(poolingObject.GetComponent<Animator>() == null)
				poolingObject.AddComponent<Animator>();
			poolingObject.GetComponent<Animator>().runtimeAnimatorController = controller.baseUI.animcontroller;

			Vector3 tempVector = poolingObject.transform.position;
			poolingObject.transform.LookAt(new Vector3(Camera.main.transform.position.x, tempVector.y, Camera.main.transform.position.z));
		}
		else
		{
			poolingObject.SetActive(true);
			poolingObject.transform.localScale = Vector3.one * SelectionController.characterScale;
		}

		pelvisGameObject = poolingObject.GetComponentsInChildren<Transform>().FirstOrDefault(p => p.name == "pelvis").gameObject;
		// m_avg_Pelvis
		Assert.IsNotNull(pelvisGameObject);
		controller.LoadMeshData(pelvisGameObject);
		SelectionController.currentModel = model;

		string model_name = model.name;

		if (model_name == "smpl" || model_name == "smpl_clone")
			SelectionController.currentMotion = controller.tPose.motion;
		else
			SelectionController.currentMotion = controller.tPose.mixamo;

		controller.SetPose(SelectionController.currentMotion.data[0]); 
		controller.Pause();
		}
	GameObject pelvisGameObject;
	}
}