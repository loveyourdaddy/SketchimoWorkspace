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
        public string characterName;
        public string motionName;
        public int totalFrame;
        public int[] collisionFrame;
        public int numberofVertex;
        public float fps = 60;
        public Quaternion[] rotation;
        public Vector3[] position;
        public Vector3[] vertices;
        public int[] vertexIndicesArray0; // left hand indices 
        public int[] vertexIndicesArray1; // right hand indices 
        // public List: smallest distance의 pair vertex indices는 python에서 찾자. 
        // 여기서 해도 되는데, python에서 어차피 vertex position을 들고 있어야해. 
    }

    public class JsonTest : MonoBehaviour
    {
        public MotionInfo motionInfo;
        public GameObject man;
        private SkinnedMeshRenderer skin;
        private Mesh mesh;
        [HideInInspector]
        public JsonMotion jsonMotion;
        private bool isUpdated = false;
        public BoxCollider box;
        public int[] vertexIndicesArray0;
        public int[] vertexIndicesArray1;
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
