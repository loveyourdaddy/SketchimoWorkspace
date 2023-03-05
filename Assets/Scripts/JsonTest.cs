using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Sketchimo.Models
{
    // Class to save JsonClass 
    [System.Serializable]
    public class JsonClass
    {
        // character infomation 
        public string characterName;

        // motion infomation 
        public string motionName;
        public int totalFrame;

        // public int numberofVertex;
        public float fps = 60;

        // joint infomations
        public int[] jointIndices;
        public int[] parentIndices;
        public string[] jointNames;

        public Quaternion[] rotation;
        public Vector3[] position;
        public Vector3[] rootPos;
        public Vector3[] boneOffsets;

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
        public CharInfo charInfo;
        [HideInInspector]
        public JsonClass jsonClass;

        // aabb
        [HideInInspector]
        public Vector3 box1_maxBound;
        [HideInInspector]
        public Vector3 box1_minBound;
        [HideInInspector]
        public Vector3 box2_maxBound;
        [HideInInspector]
        public Vector3 box2_minBound;

        private bool is_save = false;
        void Start()
        {
            if (is_save)
                SaveJson();
            else
                UpdateMotionFromJson();
        }

        // unity -> python
        public void SaveJson()
        {
            // update motion data to json
            var refMotion = motionInfo.refMotion;
            int numPose = refMotion.data.Count;
            int numJoint = refMotion.data[0].joints.Length;

            jsonClass = new JsonClass();
            jsonClass.characterName = refMotion.characterName;
            jsonClass.motionName = refMotion.motionName;
            jsonClass.totalFrame = refMotion.totalFrame;
            jsonClass.rotation = new Quaternion[numPose * numJoint];
            jsonClass.position = new Vector3[numPose * numJoint];
            jsonClass.boneOffsets = new Vector3[numJoint];
            jsonClass.jointIndices = new int[numJoint]; 
            jsonClass.parentIndices = new int[numJoint];
            jsonClass.jointNames = new string[numJoint];

            List<Utils.PoseData> refPoses = refMotion.data;
            for (int i = 0; i < numPose; i++)
            {
                Utils.PoseData refPose = refPoses[i];
                for (int j = 0; j < numJoint; j++)
                {
                    Utils.JointData refJoint = refPose.joints[j];
                    jsonClass.rotation[i * numJoint + j] = refJoint.rotation;
                    jsonClass.position[i * numJoint + j] = refJoint.position;
                }
            }
            for (int j = 0; j < numJoint; j++)
            {
                Utils.PoseData pose = refPoses[0];
                Utils.JointData joint = pose.joints[j];
                jsonClass.jointIndices[j] = joint.jointIdx;
                jsonClass.parentIndices[j] = joint.parentIdx;
                jsonClass.jointNames[j] = joint.jointName;
                jsonClass.boneOffsets[j] = charInfo.boneOffset[j];
            }

            // Set motion start 
            GetComponent<Controllers.MotionController>().SetPlayState(0);
        }

        public void Update()
        {
            // Save json when animation finish
            if (is_save)
                if (GetComponent<Controllers.MotionController>().CurrentPlay == 0 &&
                    motionInfo.GetCurrentFrame() >= motionInfo.GetTotalFrame() - 1)
                {
                    Debug.Log("Total frame: " + motionInfo.GetTotalFrame().ToString());
                    Debug.Log(motionInfo.GetCurrentFrame().ToString());

                    // set stop
                    GetComponent<Controllers.MotionController>().SetPlayState(1);

                    // save json
                    string jsonFile = JsonUtility.ToJson(jsonClass);
                    File.WriteAllText(Application.dataPath + "/Json/source_clapping.json", jsonFile); // source_TPose

                    Debug.Log("Finish to save json");
                }
        }

        // python -> unity
        public void UpdateMotionFromJson()
        {
            // parse json 
            string path = "Assets/Json/PythonOutput_clapping.json"; // PythonOutput_TPose
            string jsonString = File.ReadAllText(path);
            JsonClass jsonClass = JsonUtility.FromJson<JsonClass>(jsonString);

            Debug.Log("Motion from python:" + path);

            // reference motion
            var refMotion = motionInfo.refMotion;
            int numPose = refMotion.data.Count;
            int numJoint = refMotion.data[0].joints.Length;
            List<Utils.PoseData> refPoses = refMotion.data;

            // update motion 
            Utils.MotionData motionData = motionInfo.motion;
            var totalFrame = numPose;
            int numberOfJoint = numJoint;

            for (var i = 0; i < totalFrame; i++)
            {
                Utils.PoseData pose = refPoses[i];
                for (var j = 0; j < numberOfJoint; j++)
                {
                    Quaternion globalRot = jsonClass.rotation[i * numberOfJoint + j];
                    pose.joints[j].rotation = globalRot;
                    Vector3 position = jsonClass.position[i * numberOfJoint + j];
                    pose.joints[j].position = position;
                }
            }

            // Set motion start 
            GetComponent<Controllers.MotionController>().SetPlayState(0);
        }
    }
}
