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
        public float fps = 60;
        public Quaternion[] rotation;
        public Vector3[] position;
        public Vector3[] vertices; 
        

    }

    public class JsonTest : MonoBehaviour
    {
        public MotionInfo motionInfo;
        public GameObject man;
        private SkinnedMeshRenderer skin;
        private Mesh mesh;
        private JsonMotion jsonMotion;
        void Awake()
        {         
            // ONLY CHANGE THIS
            SaveJson();
            // UpdateMotionFromJson();
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

            // save mesh information 
            skin = man.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            mesh = skin.sharedMesh;
            jsonMotion.vertices = new Vector3[mesh.vertexCount  * motionInfo.GetTotalFrame()]; 

            // Set motion start 
            GetComponent<Controllers.MotionController>().SetPlayState(0);
        }

        public void Update()
        {
            if(motionInfo.GetCurrentFrame() >= motionInfo.GetTotalFrame() - 1)
            {
                // set stop
                GetComponent<Controllers.MotionController>().SetPlayState(1);

                // save json
                string jsonFile = JsonUtility.ToJson(jsonMotion);
                File.WriteAllText(Application.dataPath + "/UnityOutput.json", jsonFile);
            }

            // update data 
            int count = mesh.vertexCount;
            int numberofVertex = mesh.vertexCount;
            Debug.Log(motionInfo.GetCurrentFrame().ToString());
            for (int i = 0; i < count; i++)
            {
                jsonMotion.vertices[numberofVertex * (motionInfo.GetCurrentFrame() - 1) + i] = mesh.vertices[i];
            }
        }

        // Update unity motion (motionInfo) from python
        public void UpdateMotionFromJson()
        {
            // parse json 
            string path = "Assets/PythonOutput.json";
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
