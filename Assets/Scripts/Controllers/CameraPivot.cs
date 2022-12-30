using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sketchimo.Controllers
{ 
public class CameraPivot : MonoBehaviour
{
    public float speed;
    public GameObject cam;
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float m = Input.GetAxis("Mouse ScrollWheel");

        Vector3 v_result = transform.eulerAngles + new Vector3(-v * Time.deltaTime * speed, 0, 0);
        if (v_result.x > 300f || v_result.x < 30f)
            transform.eulerAngles = v_result;

        Vector3 m_result = cam.transform.localPosition + new Vector3(0, 0, -m * Time.deltaTime * speed * 3f);
        if (m_result.z > 0f && m_result.z < 4f)
            cam.transform.localPosition = m_result;
        transform.eulerAngles += new Vector3(0, -h * Time.deltaTime * speed, 0);
    }
}
}
