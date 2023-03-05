using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sketchimo.Models;

public class collsiion : MonoBehaviour
{
    public BoxCollider box;
    public JsonTest jsonTest;
    // [HideInInspector]
    // public List<int> vertexIndices;
    [HideInInspector]
    public List<int> collisionFrames;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
        GetVertexIndices();
    }
    // Get aabb and vertex indices 
    
    public void GetVertexIndices()
    {

        // Get aabb bound
        Vector3 maxBound = box.bounds.max;
        Vector3 minBound = box.bounds.min;

        if(this.name == "mixamorig:LeftHand") // left: 1
        {
            jsonTest.jsonClass.box1_maxBound = box.bounds.max;
            jsonTest.jsonClass.box1_minBound = box.bounds.min;
        }
        else
        {
            jsonTest.jsonClass.box2_maxBound = box.bounds.max;
            jsonTest.jsonClass.box2_minBound = box.bounds.min;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider collider)
    {
        // var a = collision.contacts;
        var motionInfo = jsonTest.motionInfo;
        int collisionFrame = motionInfo.GetCurrentFrame();
        Debug.Log("collision detection at " + collisionFrame.ToString());

        // jsonFile.jsonClass.frame 저장        
        collisionFrames.Add(collisionFrame);

        // // run at last time 
        jsonTest.jsonClass.collisionFrames = collisionFrames.ToArray();
    }
}
