using System;
using UnityEngine;

namespace Sketchimo.Models
{
public class CameraInfo : MonoBehaviour
{
    public Camera cam;
    public float yaw;
    public float pitch;
    public float distance;
    public float minDistance;
    public float maxDistance;
    
    public Vector3 RootCameraLocation;
    public Quaternion RootCameraRotation;
    public Vector3 CameraLocation; // Demo Camera location 
    public Vector3 CameraUpVector;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        CameraLocation = cam.transform.position;
        CameraUpVector = cam.transform.up;
    }
}
}