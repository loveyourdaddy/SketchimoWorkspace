using Sketchimo.Models;
using UnityEngine;
using Utils;

namespace Sketchimo.Controllers
{
public class CharController : MonoBehaviour
{
    public CharInfo charModel;
    public GameObject model;
    public GameObject pelvis;


    // Start is called before the first frame update
    private void Start()
    {
        charModel = GetComponent<CharInfo>();

        SketchimoCore.Instance.charModel = charModel;

        SetModel();

        charModel.SetMesh(pelvis);
    }

    public void SetPose(PoseData pose)
    {
        charModel.SetPose(pose);
    }

    public void SetModel()
	{
        if(SelectionController.currentModel != null)
		{
            Destroy(model.transform.GetChild(0).gameObject);
            GameObject instance = Instantiate(SelectionController.currentModel);

            instance.transform.parent = model.transform;
            instance.transform.localPosition = Vector3.zero;

            var childTransforms = instance.GetComponentsInChildren<Transform>();
            foreach(Transform t in childTransforms)
			{
                if(t.name == "pelvis") // m_avg_Pelvis
                {
                    pelvis = t.gameObject;
				}
			}
            
            if (SelectionController.currentTexture != null)
            {
                SkinnedMeshRenderer renderer = instance.transform.Find("SMPLX-mesh-neutral").GetComponent<SkinnedMeshRenderer>(); // m_avg
                renderer.material.mainTexture = SelectionController.currentTexture;
            }
        }
    }
}
}