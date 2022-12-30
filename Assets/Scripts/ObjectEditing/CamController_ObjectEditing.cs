using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditing
{
public class CamController_ObjectEditing : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;
    public static bool isStop = false;

    // Update is called once per frame
    void Update()
    {
        if (!isStop)
        {
            if (Input.GetMouseButton(1))
            {
                PositionSet();
                RotationSet();
            }
            Wheel();
        }
    }

    public void Wheel()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * wheel * Time.deltaTime * moveSpeed * 100f);
    }

    public void PositionSet()
    {
        float move_h = Input.GetAxisRaw("Horizontal");
        float move_v = Input.GetAxisRaw("Vertical");
        float move_d = Input.GetAxisRaw("Depth");

        Vector3 move_vec = Vector3.forward * move_v + Vector3.right * move_h + Vector3.up * move_d;
        transform.Translate(move_vec * Time.deltaTime * moveSpeed);
    }

    public void RotationSet()
    {
        float rot_h = Input.GetAxis("Horizontal");
        float rot_v = Input.GetAxis("Vertical");

        transform.Rotate(0f, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0f, Space.World);
        transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed, 0f, 0f);
    }
}
}    