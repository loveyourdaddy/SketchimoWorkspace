using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sketchimo.Models
{
public class SketchInfo : MonoBehaviour
{
    public Action OnCurrentSpaceChanged;

    public Action OnCurrentModeChanged;


    public List<Vector2> sketchTrajectory2D;
    public List<Vector3> sketchTrajectory3D;

    public static float Alpha=0.1f; // Dynamic warping parameter

    public float alphaLowerBound = 0.0f;

    public SketchSpace GetCurrentSpace()
    {
        return _currentSpace;
    }

    public void SetCurrentSpace(SketchSpace space)
    {
        _currentSpace = space;

        //OnCurrentSpaceChanged?.Invoke();
        SketchimoCore.Instance.RetrieveTrajectory3D();
    }

    public SketchMode GetCurrentMode()
    {
        return _currentMode;
    }

    public void SetCurrentMode(SketchMode mode)
    {
        _currentMode = mode;
        OnCurrentModeChanged?.Invoke();
    }

    public void ResetSketchTrajectory()
    {
        sketchTrajectory2D.Clear();
        sketchTrajectory3D.Clear();
    }

    public float GetAlpha()
    {
        return Alpha;
    }

    public void SetAlpha(float alpha)
    {
        Alpha = alpha;
    }


    public void SetFolFrame(int FrameNumber)
    {
        if (FrameNumber <= 0)
        {
            return;
        }

        _folRange = FrameNumber;
        var motionModel = SketchimoCore.Instance.motionModel;
        var endFrame = motionModel.GetCurrentFrame() + FrameNumber;
        motionModel.trajectoryEndFrame = Mathf.Min(endFrame, motionModel.GetTotalFrame());
    }

    public void SetPrevFrame(int FrameNumber)
    {
        if (FrameNumber <= 0)
        {
            return;
        }

        _prevRange = FrameNumber;
        var motionModel = SketchimoCore.Instance.motionModel;
        var startFrame = motionModel.GetCurrentFrame() - FrameNumber;
        motionModel.trajectoryStartFrame = Mathf.Max(startFrame, 0);
	}

    public int GetPrevFrame()
    {
        return _prevRange;
    }

    public int GetFolFrame()
    {
        return _folRange;
    }

    public int GetSketchTrajectory2DNum()
    {
        return sketchTrajectory2D.Count;
    }

    public int GetSketchTrajectory3DNum()
    {
        return sketchTrajectory3D.Count;
    }

    private SketchSpace _currentSpace = SketchSpace.Global;
    private SketchMode _currentMode = SketchMode.Select;

    // TODO: Move this to MotionInfo
    private int _prevRange = 10;
    private int _folRange = 10;
}
}