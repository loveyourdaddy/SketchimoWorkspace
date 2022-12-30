using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Utils
{
    public class MovementLimitInfo : MonoBehaviour
    {
        public struct JointLimitData
        {
            Vector3 Min;
            Vector3 Max;
            public JointLimitData(Vector3 min, Vector3 max) {
                Min = min;
                Max = max;
            }
            public Vector3 ClampVector(Vector3 index) {
                return new Vector3(Mathf.Clamp(index.x, Min.x, Max.x),
                    Mathf.Clamp(index.y, Min.y, Max.y),
                    Mathf.Clamp(index.z, Min.z, Max.z));                
            }
        };

        public Dictionary<string, JointLimitData> JointDataArray;
        public void AddJointData(string name, Vector3 min, Vector3 max)
        {
            JointDataArray.Add(name, new JointLimitData(min, max));
        }

        private void Start()
        {
            JointDataArray = new Dictionary<string, JointLimitData>();
            //Legs
            AddJointData("m_avg_L_Hip", new Vector3(-90.0f, -60.0f, -60.0f), new Vector3(30.0f, 40.0f, 60.0f));
            AddJointData("m_avg_R_Hip", new Vector3(-90.0f, -40.0f, -60.0f), new Vector3(30.0f, 60.0f, 60.0f));
            AddJointData("m_avg_L_Knee", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(150.0f, 0.0f, 0.0f));
            AddJointData("m_avg_R_Knee", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(150.0f, 0.0f, 0.0f));
            AddJointData("m_avg_L_Ankle", new Vector3(-50.0f, -30.0f, -50.0f), new Vector3(50.0f, 30.0f, 50.0f));
            AddJointData("m_avg_R_Ankle", new Vector3(-50.0f, -30.0f, -50.0f), new Vector3(50.0f, 30.0f, 50.0f));
            AddJointData("m_avg_L_Foot", new Vector3(-180.0f, -180.0f, -180.0f), new Vector3(180.0f, 180.0f, 180.0f));
            AddJointData("m_avg_R_Foot", new Vector3(-180.0f, -180.0f, -180.0f), new Vector3(180.0f, 180.0f, 180.0f));
            //Arms
            AddJointData("m_avg_L_Collar", new Vector3(-15.0f, -15.0f, -15.0f), new Vector3(30.0f, 0.0f, 15.0f));
            AddJointData("m_avg_R_Collar", new Vector3(-15.0f, 0.0f, -15.0f), new Vector3(30.0f, 15.0f, 15.0f));
            AddJointData("m_avg_L_Shoulder", new Vector3(0.0f, -90.0f, -90.0f), new Vector3(0.0f, 90.0f, 90.0f));
            AddJointData("m_avg_R_Shoulder", new Vector3(0.0f, -90.0f, -90.0f), new Vector3(0.0f, 90.0f, 90.0f));
            AddJointData("m_avg_L_Elbow", new Vector3(-90.0f, 0.0f, 0.0f), new Vector3(90.0f, 180.0f, 0.0f));
            AddJointData("m_avg_R_Elbow", new Vector3(-90.0f, -180.0f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f));
            AddJointData("m_avg_L_Wrist", new Vector3(-70.0f, -20.0f, -70.0f), new Vector3(70.0f, 20.0f, 70.0f));
            AddJointData("m_avg_R_Wrist", new Vector3(-70.0f, -20.0f, -70.0f), new Vector3(70.0f, 20.0f, 70.0f));
            AddJointData("m_avg_L_Hand", new Vector3(-180.0f, -180.0f, -180.0f), new Vector3(180.0f, 180.0f, 180.0f));
            AddJointData("m_avg_R_Hand", new Vector3(-180.0f, -180.0f, -180.0f), new Vector3(180.0f, 180.0f, 180.0f));

            //Head
            AddJointData("m_avg_Neck", new Vector3(-40.0f, -40.0f, -40.0f), new Vector3(40.0f, 40.0f, 40.0f));
            AddJointData("m_avg_Head", new Vector3(-40.0f, -40.0f, -40.0f), new Vector3(40.0f, 40.0f, 40.0f));
            //Spine
            AddJointData("m_avg_Spine1", new Vector3(-30.0f, -60.0f, -30.0f), new Vector3(30.0f, 60.0f, 30.0f));
            AddJointData("m_avg_Spine2", new Vector3(-30.0f, -60.0f, -30.0f), new Vector3(30.0f, 60.0f, 30.0f));
            AddJointData("m_avg_Spine3", new Vector3(-30.0f, -60.0f, -30.0f), new Vector3(30.0f, 60.0f, 30.0f));
            AddJointData("pelvis", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
            // m_avg_Pelvis
        }
    }
    
    
}