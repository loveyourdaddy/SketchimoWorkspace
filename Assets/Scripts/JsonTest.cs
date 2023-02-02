using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Sketchimo.Models
{
    // JsonMotion for serializable
    [System.Serializable]
    public class JsonMotion
    {
        public string characterName;
        public string motionName;
        public int totalFrame;
        public int numberofVertex;
        public float fps = 60;
        public Quaternion[] rotation;
        public Vector3[] position;
        public Vector3[] vertices;
        public int[] vertexIndicesArray;
    }

    public class JsonTest : MonoBehaviour
    {
        public MotionInfo motionInfo;
        public GameObject man;
        private SkinnedMeshRenderer skin;
        private Mesh mesh;
        private JsonMotion jsonMotion;
        private bool isUpdated = false;
        public BoxCollider box;
        // public int vertexIndices[];
        public List<int> vertexIndices;
        public int[] vertexIndicesArray;
        void Awake()
        {
            // Get aabb and vertex indices 
            // GetVertexIndices();

            // ONLY CHANGE THIS
            SaveJson();
            // UpdateMotionFromJson();
        }

        void printRecursive(Transform currBone)
        {
            for (int i = 0; i < currBone.childCount; i++)
            {
                Debug.Log(currBone.name);
                Debug.Log(currBone.transform.position.ToString("F4"));
                var childBone = currBone.GetChild(i);
                printRecursive(childBone);
                Debug.Log(i);
            }
        }

        // void Start()
        // {

        // }
        public void GetVertexIndices()
        {

            // Get aabb bound
            Vector3 maxBound = box.bounds.max;
            Vector3 minBound = box.bounds.min;

            // Get vertices in bound
            Transform ybot = man.transform.GetChild(0);
            SkinnedMeshRenderer skin = ybot.GetComponentInChildren<SkinnedMeshRenderer>();

            // mesh in T pose 
            Mesh mesh = skin.sharedMesh;
            // mesh.vertices;

            // check it is T pose 
            // Transform root = ybot.Find("mixamorig:Hips");
            // printRecursive(root);

            // get vertices indices 
            // bound안에 있는 index을 리스트 

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                if (maxBound.x > mesh.vertices[i].x && minBound.x < mesh.vertices[i].x
                    && maxBound.y > mesh.vertices[i].y && minBound.y < mesh.vertices[i].y
                    && maxBound.z > mesh.vertices[i].z && minBound.z < mesh.vertices[i].z)
                {
                    vertexIndices.Add(i);
                }
            }

            vertexIndicesArray = vertexIndices.ToArray();
        }

        public void SaveJson()
        {
            var refMotion = motionInfo.refMotion;
            int numPose = refMotion.data.Count;
            int numJoint = refMotion.data[0].joints.Length;

            jsonMotion = new JsonMotion();
            jsonMotion.rotation = new Quaternion[numPose * numJoint];
            jsonMotion.position = new Vector3[numPose * numJoint];

            jsonMotion.characterName = refMotion.characterName;
            jsonMotion.motionName = refMotion.motionName;
            jsonMotion.totalFrame = refMotion.totalFrame;
            List<Utils.PoseData> refPoses = refMotion.data;

            for (int i = 0; i < numPose; i++)
            {
                Utils.PoseData refPose = refPoses[i];
                for (int j = 0; j < numJoint; j++)
                {
                    Utils.JointData refJoint = refPose.joints[j];
                    jsonMotion.rotation[i * numJoint + j] = refJoint.rotation;
                    jsonMotion.position[i * numJoint + j] = refJoint.position;
                }
            }

            // // save mesh information 
            // skin = man.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            // mesh = skin.sharedMesh;
            // jsonMotion.numberofVertex = mesh.vertexCount;
            // jsonMotion.vertices = new Vector3[mesh.vertexCount * motionInfo.GetTotalFrame()];
            // jsonMotion.vertexIndicesArray = vertexIndicesArray;

            // // Set motion start 
            // GetComponent<Controllers.MotionController>().SetPlayState(0);
        }

        public void Update()
        {
            if(GetComponent<Controllers.MotionController>().CurrentPlay == 0)
            {
            //     int count = mesh.vertexCount;
            //     int numberofVertex = mesh.vertexCount;
                Debug.Log("frame: " + motionInfo.GetCurrentFrame());
                // for (int i = 0; i < count; i++)
                // {
                //     jsonMotion.vertices[numberofVertex * (motionInfo.GetCurrentFrame()) + i] = mesh.vertices[i]; // - 1
                // }

                if(motionInfo.GetCurrentFrame() >= motionInfo.GetTotalFrame() - 1) // && isUpdated == false
                {
                    Debug.Log("Total frame: " + motionInfo.GetTotalFrame().ToString());
                    Debug.Log(motionInfo.GetCurrentFrame().ToString());

                    // set stop
                    GetComponent<Controllers.MotionController>().SetPlayState(1);

                    // save json
                    string jsonFile = JsonUtility.ToJson(jsonMotion);
                    File.WriteAllText(Application.dataPath + "/Json/UnityOutput_clapping.json", jsonFile); // UnityOutput_Tpose
                    // isUpdated = true;
                }
            }

            // find hand joint 
            // Transform hip = man.transform.GetChild(0).transform.GetChild(2); // LeftHand
            // var leftHand = hip.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand");
            // // var tmmmp = leftHand.gameObject.GetComponent<>;

            // Vector3 center = leftHand.transform.position;
            // Vector3 extents = Vector3.one;

            // //find position of last vertex index 
            // // 가장 오른쪽에 있는 vertex index을 찾아보자. -> python
            // Transform ybot_trf = man.transform.GetChild(0);
            // Transform alpha_surface_trf = ybot_trf.GetChild(0);
            // var render = alpha_surface_trf.GetComponent<SkinnedMeshRenderer>();
            // var mesh = alpha_surface_trf.GetComponent<Mesh>();

            // // find min x 
            // var verteices = mesh.vertices;
            // float min_x = 10;
            // int min_index = 0;
            // for(int i=0; i<mesh.vertexCount; i++)
            // {
            //     if(min_x < verteices[i].x)
            //     {
            //         min_x = verteices[i].x;
            //         min_index = i;
            //     }
            // }
        }

        void OnDrawGizmos()
        {
            // bounds.center = center;


            // find vertices index


            // Bounds bounds = render.bounds;
            // Gizmos.matrix = Matrix4x4.identity;
            // Gizmos.color = Color.blue;
            // Gizmos.DrawWireCube(bounds.center, bounds.extents*2);
        }

        // Update unity motion (motionInfo) from python
        public void UpdateMotionFromJson()
        {
            // parse json 
            string path = "Assets/Json/PythonOutput.json";
            string jsonString = File.ReadAllText(path);
            JsonMotion jsonMotion = JsonUtility.FromJson<JsonMotion>(jsonString);

            // update motion 
            Utils.MotionData motionData = motionInfo.motion;
            motionData.characterName = jsonMotion.characterName;
            motionData.motionName = jsonMotion.motionName;
            motionData.totalFrame = jsonMotion.totalFrame;
            motionData.fps = jsonMotion.fps;

            motionData.data = new List<Utils.PoseData>();
            int numberOfJoint = (int)(jsonMotion.rotation.Length / jsonMotion.totalFrame);
            for (var i = 0; i < jsonMotion.totalFrame; i++)
            {
                Utils.PoseData pose = new Utils.PoseData();
                pose.joints = new Utils.JointData[numberOfJoint];
                for (var j = 0; j < numberOfJoint; j++)
                {
                    pose.joints[j].rotation = jsonMotion.rotation[i * numberOfJoint + j];
                    pose.joints[j].position = jsonMotion.position[i * numberOfJoint + j];
                }
                motionData.data.Add(pose);
            }
        }
    }
}
