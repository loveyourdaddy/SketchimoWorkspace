using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditing
{
public class Scaler : MonoBehaviour
{
    void Update()
    {
        float scale = Vector3.Distance(Camera.main.gameObject.transform.position, gameObject.transform.position) / 20f;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
}