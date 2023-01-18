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
    }

    public class JsonTest : MonoBehaviour
    {
        public MotionInfo motionInfo;
        void Awake()
        {
            // SaveJson();
            UpdateMotionFromJson();
        }

        public void SaveJson()
        {
            var refMotion = motionInfo.refMotion; 
            int numPose = refMotion.data.Count;
            int numJoint = refMotion.data[0].joints.Length;

            JsonMotion motion = new JsonMotion();
            motion.rotation = new Quaternion[numPose * numJoint];
            motion.position = new Vector3[numPose * numJoint];

            motion.characterName = refMotion.characterName;
            motion.motionName = refMotion.motionName;
            motion.totalFrame = refMotion.totalFrame;
            List<Utils.PoseData> refPoses = refMotion.data;

            for (int i = 0; i < numPose; i++)
            {
                Utils.PoseData refPose = refPoses[i];
                for (int j = 0; j < numJoint; j++)
                {
                    Utils.JointData refJoint = refPose.joints[j];
                    motion.rotation[i * numJoint + j] = refJoint.rotation;
                    motion.position[i * numJoint + j] = refJoint.position;
                }
            }

            string jsonFile = JsonUtility.ToJson(motion);
            File.WriteAllText(Application.dataPath + "/UnityOutput.json", jsonFile);
        }

        // Update motionInfo(MotionData class) from motionData
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
