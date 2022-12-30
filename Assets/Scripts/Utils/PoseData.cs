using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
[Serializable]
public struct PoseData
{
    public int frameNumber;
    public Vector3 centerOfMass;
    public Vector3 centerOfMassVelocity;
    public JointData[] joints;

    public PoseData(int nubmerOfJoints)
    {
        frameNumber = -1;
        centerOfMass = default;
        centerOfMassVelocity = default;
        joints = new JointData[nubmerOfJoints];
    }

    public PoseData(PoseData data)
    {
        frameNumber = data.frameNumber;
        centerOfMass = data.centerOfMass;
        centerOfMassVelocity = data.centerOfMassVelocity;
        joints = new JointData[data.joints.Length];
        Array.Copy(data.joints, joints, joints.Length);
    }

    public Vector3 GetWorldPositionOf(int index)
    {
        if (index >= joints.Length)
        {
            Debug.LogError($"Out of index");
            return Vector3.zero;
        }

        var rootPos = joints[0].position;
        var rootRot = joints[0].rotation;

        // other joint position and rotation is defined by root
        // But root pos and rot is global value 
        if (index == 0)
            return rootPos;

        return rootPos + rootRot * joints[index].position;
    }

    public Quaternion GetWorldRotationOf(int index)
    {
        if (index >= joints.Length)
        {
            Debug.LogError($"Out of index");
            return Quaternion.identity;
        }

        if (index == 0)
        {
            return joints[index].rotation;
        }

        var rootRot = joints[0].rotation;
        return rootRot * joints[index].rotation;
    }

    public void SetRotationFromWorldRotation(int index, Quaternion worldRotation)
    {
        if (index >= joints.Length)
        {
            Debug.LogError($"Out of index");
            return;
        }

        if (index == 0)
        {
            joints[index].rotation = worldRotation;
        }

        var rootRot = joints[0].rotation;
        joints[index].rotation = Quaternion.Inverse(rootRot) * worldRotation;
    }

    public List<Vector3> GetWorldPositions()
    {
        var positions = new List<Vector3>(joints.Length);
        for (int i = 0; i < joints.Length; i++)
            positions.Add(GetWorldPositionOf(i));

        return positions;
    }

    public List<Quaternion> GetWorldRotations()
    {
        var rotations = new List<Quaternion>(joints.Length);
        for (int i = 0; i < joints.Length; i++)
            rotations.Add(GetWorldRotationOf(i));

        return rotations;
    }

    public List<Quaternion> GetLocalRotations()
    {
        var rotations = new List<Quaternion>(joints.Length);
        for (int i = 0; i < joints.Length; i++)
        {
            var parentIdx = joints[i].parentIdx;
            Quaternion localRotation;
            if (i == 0)
                localRotation = joints[0].rotation;
            else
                localRotation = Quaternion.Inverse(joints[parentIdx].rotation) 
                        * joints[i].rotation;
            
            rotations.Add(localRotation);
        }

        return rotations;
    }

    public Quaternion GetLocalRotationOf(int index)
    {
        var parentRotation = joints[joints[index].parentIdx].rotation;
        var localRotation = Quaternion.Inverse(parentRotation) * joints[index].rotation;
        return localRotation;
    }

    public void RotateJoint(int jointIndex, Quaternion delta)
    {
        joints[jointIndex].rotation = delta * joints[jointIndex].rotation;
        if (jointIndex == 0)
        {
            return;
        }
        
        foreach (var childIndex in joints[jointIndex].childrenIdx)
        {
            var childPos = joints[childIndex].position - joints[jointIndex].position;
            var deltaPos = delta * childPos - childPos;
            RotateJoint(childIndex, delta);
            TranslateJoint(childIndex, deltaPos);
        }
    }

    public void ClampJoint(int index, MovementLimitInfo movementLimitInfo)
    {        
        if (index == 0)
            return;
        // change global orientation to local orientation
        int parentIndex = joints[index].parentIdx;
        var parentJointRot = joints[parentIndex].rotation;
        var quatLocalRot = Quaternion.Inverse(parentJointRot) * joints[index].rotation;
        var eulerLocalRot = quatLocalRot.eulerAngles;

        if(!movementLimitInfo.JointDataArray.ContainsKey(joints[index].jointName)) // TryGetValue(joints[index].jointName)
            return;
        var angleLimitData = movementLimitInfo.JointDataArray[joints[index].jointName];
        angleLimitData.ClampVector(eulerLocalRot);

        foreach (var childIndex in joints[index].childrenIdx)
        {
            ClampJoint(childIndex, movementLimitInfo);
        }
    }

    public void RotateJointLocal(int jointIndex, Quaternion delta)
    {
        if (jointIndex == 0)
        {
            RotateJoint(jointIndex, delta);
            return;
        }

        var parentRotation = joints[joints[jointIndex].parentIdx].rotation;
        var parentInverseRotation = Quaternion.Inverse(parentRotation);

        RotateJoint(jointIndex, parentRotation * delta * parentInverseRotation);
    }

    public void TranslateJoint(int index, Vector3 delta)
    {
        joints[index].position += delta;

        foreach (var i in joints[index].childrenIdx)
            TranslateJoint(i, delta);
    }
}
}