using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sketchimo.Models;

public class collsiion : MonoBehaviour
{
    public BoxCollider box;
    public JsonTest jsonTest;
    [HideInInspector]
    public List<int> vertexIndices;
    public List<int> collisionFrames;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
        // 각 collision box 안의 vertex index을 가지고 있자. 
        GetVertexIndices();
    }
    // Get aabb and vertex indices 
    
    public void GetVertexIndices()
    {

        // Get aabb bound
        Vector3 maxBound = box.bounds.max;
        Vector3 minBound = box.bounds.min;

        // Get vertices in bound
        Transform ybot = jsonTest.man.transform.GetChild(0);
        SkinnedMeshRenderer skin = ybot.GetComponentInChildren<SkinnedMeshRenderer>();

        // mesh in T pose 
        Mesh mesh = skin.sharedMesh;

        // get vertices indices: bound안에 있는 index을 리스트 
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (maxBound.x > mesh.vertices[i].x && minBound.x < mesh.vertices[i].x
                && maxBound.y > mesh.vertices[i].y && minBound.y < mesh.vertices[i].y
                && maxBound.z > mesh.vertices[i].z && minBound.z < mesh.vertices[i].z)
            {
                vertexIndices.Add(i);
            }
        }
        if(box.transform.name == "mixamorig:LeftHand")
            jsonTest.jsonMotion.vertexIndicesArray0 = vertexIndices.ToArray();
        else // RightHane
            jsonTest.jsonMotion.vertexIndicesArray1 = vertexIndices.ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        // var a = collision.contacts;
        var motionInfo = jsonTest.motionInfo;
        int collisionFrame = motionInfo.GetCurrentFrame();
        Debug.Log("collision detection at " + collisionFrame.ToString());

        // jsonFile.jsonMotion.frame 저장        
        collisionFrames.Add(collisionFrame);

        // // run at last time 
        // if(motionInfo.GetCurrentFrame() >= motionInfo.GetTotalFrame() - 1)
        jsonTest.jsonMotion.collisionFrame = collisionFrames.ToArray();

        // vertex pair 저장.
        // collision이 발생했을 때, vertex indices 사이에 가장 가까운 vertex을 찾자. 
    }

    void OnApplicatoinQuit()
    {
        jsonTest.jsonMotion.collisionFrame = collisionFrames.ToArray();
    }
}
