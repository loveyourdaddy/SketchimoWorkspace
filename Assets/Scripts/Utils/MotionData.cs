using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils
{
public class MotionData : ScriptableObject
{
    public string characterName;
    public string motionName;
    public int totalFrame;
    public float fps = 60;
    public List<PoseData> data;

    public void Init(MotionData source)
    {
        characterName = source.characterName;
        motionName = source.motionName;
        totalFrame = source.totalFrame;
        fps = source.fps;
        data = new List<PoseData>(source.data.Count);
        for (var i = 0; i < source.data.Count; i++)
            data.Add(new PoseData(source.data[i]));
    }

    public void Init(int frameCount)
    {
        data = new List<PoseData>(frameCount);
    }

    public void Save()
    {
#if UNITY_EDITOR
        var path = $"Assets/output/{motionName}.motion.asset";
        Debug.Log($"Saving to {path}");
        if (!AssetDatabase.IsValidFolder("Assets/output"))
        {
            AssetDatabase.CreateFolder("Assets", "output");
        }

        AssetDatabase.CreateAsset(this, path);
        AssetDatabase.SaveAssets();
#endif // UNITY_EDITOR
    }
}
}