using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
[Serializable]
public struct JointData
{
    public int jointIdx;
    public int parentIdx;
    public List<int> childrenIdx;
    public string jointName;
    public Vector3 position;
    public Quaternion rotation;
    public Quaternion inverseRotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public JointData(int idx)
    {
        parentIdx = -1;
        jointIdx = idx;
        jointName = default;
        childrenIdx = new List<int>();
        position = default;
        rotation = default;
        inverseRotation = default;
        velocity = default;
        angularVelocity = default;
    }
}
}