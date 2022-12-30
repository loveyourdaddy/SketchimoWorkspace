using System.Collections;
using System.Collections.Generic;
using Sketchimo.Models;
using UnityEngine;

namespace Sketchimo.Controllers
{
public class CameraController : MonoBehaviour
{
    public CameraInfo cameraModel;
    // Start is called before the first frame update
    void Start()
    {
        cameraModel = GetComponent<CameraInfo>();
        SketchimoCore.Instance.cameraModel = cameraModel;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
}