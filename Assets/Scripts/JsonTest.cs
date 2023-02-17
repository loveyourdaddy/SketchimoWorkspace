using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Sketchimo.Models
{
    // Class to save JsonMotion 
    [System.Serializable]
    public class JsonMotion
    {
        // character infomation 
        public string characterName;

        // motion infomation 
        public string motionName;
        public int totalFrame;

        // public int numberofVertex;
        public float fps = 60;
        public Quaternion[] rotation;
        public Vector3[] position;

        // collision information
        public int[] collisionFrames;
        public Vector3 box1_maxBound;
        public Vector3 box1_minBound;
        public Vector3 box2_maxBound;
        public Vector3 box2_minBound;
    }

    public class JsonTest : MonoBehaviour
    {
        public MotionInfo motionInfo;
        public GameObject man;
        [HideInInspector]
        public JsonMotion jsonMotion;
        // private bool isUpdated = false;

        // aabb
        [HideInInspector]
        public Vector3 box1_maxBound;
        [HideInInspector]
        public Vector3 box1_minBound;
        [HideInInspector]
        public Vector3 box2_maxBound;
        [HideInInspector]
        public Vector3 box2_minBound;
        
        void Awake()
        {
            SaveJson();
            // UpdateMotionFromJson();
        }

        // unity -> python
        public void SaveJson()
        {
            // update motion data to json
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

            // Set motion start 
            GetComponent<Controllers.MotionController>().SetPlayState(0);
        }

        public void Update()
        {
            // Save json when animation finish
            // unity -> python
            if (GetComponent<Controllers.MotionController>().CurrentPlay == 0 &&
                motionInfo.GetCurrentFrame() >= motionInfo.GetTotalFrame() - 1)
            {
                Debug.Log("Total frame: " + motionInfo.GetTotalFrame().ToString());
                Debug.Log(motionInfo.GetCurrentFrame().ToString());

                // set stop
                GetComponent<Controllers.MotionController>().SetPlayState(1);

                // save json
                string jsonFile = JsonUtility.ToJson(jsonMotion);
                File.WriteAllText(Application.dataPath + "/Json/UnityOutput_clapping.json", jsonFile); // UnityOutput_Tpose

                Debug.Log("Finish to save json");
            }
        }

        // python -> unity
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
