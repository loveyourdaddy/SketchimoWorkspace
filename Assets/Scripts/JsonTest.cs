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
        // public Quaternion[] data;  
        public Quaternion[][] data;  

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
        void Start()
        {
            SaveJson();
        }

        public void SaveJson()
        {
            var refMotion = motionInfo.refMotion; // MotionData
            int numPose = refMotion.data.Count;
            int numJoint = refMotion.data[0].joints.Length;

            Motion motion = new Motion();
            int quatDim = 4;

            motion.characterName = refMotion.characterName;
            motion.motionName = refMotion.motionName;
            motion.totalFrame = refMotion.totalFrame;
            var refPoses = refMotion.data; // List<PoseData> 이걸 못찾는다고? var은 되는데?

            for (int j=0; j<numPose; j++) 
            {
                var refPose = refPoses[j];
                Quaternion[] pose = new Quaternion[numJoint];
                for(int i=0; i<numJoint; i++)
                {
                    var refJoint = refPose.joints[i];
                    Quaternion rot = refJoint.rotation;
                    pose[i] = rot;
                }
                motion.data[j] = pose;

                // motion.data.Add(pose);
            }

            string json = JsonUtility.ToJson(motion);
            File.WriteAllText(Application.dataPath + "/MotionData.json", json);
            motion.printData();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
