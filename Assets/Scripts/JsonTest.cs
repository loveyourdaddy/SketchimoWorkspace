using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
// using Models;
using TMPro;
// using Utils;
using UnityEngine;

namespace Sketchimo.Models
{
    [System.Serializable]
    public class Motion
    {
        public string characterName;
        public string motionName;
        public int totalFrame;
        public float fps = 60;
        public Quaternion[] data;  // []

        // public void Init(MotionData info)
        // {
        //     characterName = info.characterName;
        //     motionName = info.motionName;
        //     totalFrame = info.totalFrame;
        //     data = info.data;
        // }
        public void printData()
        {
            Debug.Log("characterName : " + characterName);
            Debug.Log("motionName : " + motionName);
            Debug.Log("totalFrame : " + totalFrame);
        }
    }

    public class JsonTest : MonoBehaviour
    {
        // Start is called before the first frame update
        public MotionInfo motionInfo;
        void Awake()
        {
            // SaveJson();
            var jsonMotion = ParseJson();
                        
            // Set motionInfo(MotionData class) from motionData
            Utils.MotionData motionData = motionInfo.motion; // new Utils.MotionData();
            motionData.characterName = jsonMotion.characterName;
            motionData.motionName = jsonMotion.motionName;
            motionData.totalFrame = jsonMotion.totalFrame;
            motionData.fps = jsonMotion.fps;

            motionData.data = new List<Utils.PoseData> (); // jsonMotion.totalFrame
            int numberOfJoint = (int) (jsonMotion.data.Length / jsonMotion.totalFrame);
            for (var i = 0; i < jsonMotion.totalFrame; i++)
            {
                Utils.PoseData pose = new Utils.PoseData();
                // Utils.PoseData pose = motionData.data[i];
                pose.joints = new Utils.JointData[numberOfJoint];
                for (var j = 0; j < numberOfJoint; j++)
                {
                    // Utils.JointData joint = pose.joints[j];
                    pose.joints[j].rotation = jsonMotion.data[i * numberOfJoint + j];
                }
                motionData.data.Add(pose);
            }            
            // motionInfo.motion = motionData;
        }

        public void SaveJson()
        {
            var refMotion = motionInfo.refMotion; // MotionData
            int numPose = refMotion.data.Count;
            int numJoint = refMotion.data[0].joints.Length;

            Motion motion = new Motion();
            motion.data = new Quaternion[numPose * numJoint];

            motion.characterName = refMotion.characterName;
            motion.motionName = refMotion.motionName;
            motion.totalFrame = refMotion.totalFrame;
            List<Utils.PoseData> refPoses = refMotion.data; 

            for (int j=0; j<numPose; j++)
            {
                Utils.PoseData refPose = refPoses[j];
                for(int i=0; i<numJoint; i++)
                {
                    Utils.JointData refJoint = refPose.joints[i];
                    Quaternion rot = refJoint.rotation;
                    motion.data[j * numJoint + i] = rot;
                }
            }

            string jsonFile = JsonUtility.ToJson(motion);
            File.WriteAllText(Application.dataPath + "/MotionData.json", jsonFile);
            motion.printData();
        }

        public Motion ParseJson()
        {
            string path = "output.json";
            string jsonString = File.ReadAllText(path); 
            Motion motion = JsonUtility.FromJson<Motion>(jsonString);
            return motion;
        }

    }
}
