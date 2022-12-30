using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Sketchimo.Models
{
public class MotionInfo : MonoBehaviour
{
    public MotionData refMotion;

    [HideInInspector]
    public MotionData motion;

    public Action OnCurrentPlayChanged;
    public Action OnMotionChanged;
    public Action OnFrameChanged;

    // Trajectory Setting
    public int trajectoryStartFrame = -1;
    public int trajectoryEndFrame = -1;

    public List<Vector3> trajectory3D;
    public List<Vector2> trajectory2D;
    public List<Vector3> targetTrajectory3D;
    public List<int> subTrajectoryIndex;
    public List<float> diffOrderedParameter;

    //Edit Velocity 
    public float velocityEditWeight = 1.1f;
    public float velocityEditWeightLowerBound = 0.0f;
    public float velocityEditWeightUpperBound = 1.2f;

    public void Awake()
    {
        motion = ScriptableObject.CreateInstance<MotionData>();
        motion.Init(refMotion);
    }

    public void SetFrame(int frameNumber)
    {
        if (_currentFrame == frameNumber)
        {
            return;
        }

        _currentFrame = frameNumber;

        if (_currentFrame > GetTotalFrame() - 1)
        {
            _currentFrame = GetTotalFrame() - 1;
        }

        OnFrameChanged?.Invoke();
    }

    public int GetTotalFrame()
    {
        return motion.totalFrame;
    }

    public int GetCurrentFrame()
    {
        return _currentFrame;
    }

    public PlayState GetCurrentPlay()
    {
        return _currentPlay;
    }

    public void SetCurrentPlay(PlayState play)
    {
        _currentPlay = play;
        OnCurrentPlayChanged?.Invoke();
    }

    public Vector3 GetJointPosition(int jointIndex)
    {
        return motion.data[_currentFrame].joints[jointIndex].position;
    }

    public PoseData GetCurrentPose()
    {
        return GetPoseAt(_currentFrame);
    }

    public PoseData GetPoseAt(int frame)
    {
        return motion.data[frame];
    }

    private PlayState _currentPlay = PlayState.Pause;
    private int _currentFrame = -1;
}
}