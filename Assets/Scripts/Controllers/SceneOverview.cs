using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sketchimo.Controllers
{
public class SceneOverview : MonoBehaviour
{
    Animator anim;
    private static SceneOverview instance = null;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static SceneOverview Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

        // Update is called once per frame
        public void OnOff()
    {
        anim.SetBool("OnOff", !anim.GetBool("OnOff"));
    }


    public void SceneChange(string name)
	{
        if (SelectionController.currentModel != null &&
                SelectionController.currentMotion &&
                SceneManager.GetActiveScene().name != name)
        {
            SceneManager.LoadScene(name);
            OnOff();
        }
	}
}
}
