using Sketchimo.Models;
using System;
using UnityEngine;

namespace Sketchimo.Controllers
{
public enum PlayState { Play = 0, Pause = 1, Backward = 2 }
public class MotionController : MonoBehaviour
{   
    [HideInInspector]
    public PlayState CurrentPlay = PlayState.Pause;
    [HideInInspector]
    public int CurrentFrame = 0;
    public int TotalFrame = 0;
    public Action OnPlayStateChanged;
    public JsonTest jsonTest;

	public void Start()
    {
        // _motionModel = GetComponent<MotionInfo>();
        // _motionModel = jsonTest.motionInfo;
        var tmp = GetComponent<JsonTest>();
        _motionModel = tmp.motionInfo;

        if (SelectionController.currentMotion != null)
            _motionModel.motion = SelectionController.currentMotion;

        _charModel = GetComponent<CharInfo>();
        _sketchModel = GetComponent<SketchInfo>();

        SketchimoCore.Instance.motionModel = _motionModel;

        _motionModel.OnFrameChanged += SetTrajectoryRange;

        TotalFrame = _motionModel.GetTotalFrame();
        SetFrameFromSlider(0);
    } 

    public void SetMotionFrame(int frame)
    {
        CurrentFrame = frame;
        _motionModel.SetFrame(frame);

        _charModel.SetPose(_motionModel.GetCurrentPose());
    }

    public void SetFrameFromSlider(float sliderValue)
    {
        SetMotionFrame((int)(sliderValue));
    }

    public void SetFrameByValue(int value)
    {
        TotalFrame = _motionModel.GetTotalFrame();
        CurrentFrame = CurrentFrame + value;

        if (CurrentFrame > TotalFrame - 1)
            CurrentFrame = 0;
        if (CurrentFrame < 0)
            CurrentFrame = TotalFrame - 1;

        SetMotionFrame(CurrentFrame);
    }

    public void JumpFrameByValue(int value)
    {
        int tempFrame = Mathf.Clamp(CurrentFrame + value, 0, _motionModel.GetTotalFrame() - 1);
        SetMotionFrame(tempFrame);
    }

    public void SetPlayState(int state)
    {
        CurrentPlay = (PlayState)state;
        OnPlayStateChanged?.Invoke();
    }

    private void SetTrajectoryRange()
    {
        var startFrame = _motionModel.GetCurrentFrame() - _sketchModel.GetPrevFrame();
        var endFrame = _motionModel.GetCurrentFrame() + _sketchModel.GetFolFrame();
        _motionModel.trajectoryStartFrame = Mathf.Max(startFrame, 0);
        _motionModel.trajectoryEndFrame = Mathf.Min(endFrame, _motionModel.GetTotalFrame());
    }

    public void OnOff(GameObject target)
	{
        target.SetActive(!target.activeSelf);
	}

    private MotionInfo _motionModel;
    private CharInfo _charModel;
    private SketchInfo _sketchModel;
}
}