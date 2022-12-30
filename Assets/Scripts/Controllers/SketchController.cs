using System;
using System.Collections.Generic;
using Sketchimo.Models;
using UnityEngine;

namespace Sketchimo.Controllers
{
public class SketchController : MonoBehaviour
{
    private CharInfo _charModel;
    private MotionInfo _motionModel;
    private SketchInfo _sketchModel;

    public void Start()
    {
        _motionModel = GetComponent<MotionInfo>();
        _sketchModel = GetComponent<SketchInfo>();
        _charModel = GetComponent<CharInfo>();

        SketchimoCore.Instance.sketchModel = _sketchModel;
    }

    public void SetDrawInput(List<Vector2> points)
    {
        if (points.Count == 0) return;

        var mode = _sketchModel.GetCurrentMode();
        if (mode == SketchMode.Select)
        {
            SketchimoCore.Instance.SelectBones(points);
        }
        else if (mode == SketchMode.VelocityEdit)
        {
// 		//MotionModel->SubTrajectory3D.Empty();
// 		MotionModel->SubTrajectoryIndex.Empty();
        }
    }

    public void SetDrawInputFinished(List<Vector2> points)
    {
        if (points.Count == 0) return;

        var mode = _sketchModel.GetCurrentMode();
        switch (mode)
        {
            case SketchMode.Select:
                //body line setting 
                SketchimoCore.Instance.SelectBones(points);
                break;
            case SketchMode.BodyLineEdit:
                if (_charModel.selectedBone != default)
                {
                    _sketchModel.sketchTrajectory2D = points;
                    SketchimoCore.Instance.BodyLineEdit();
                }
                break;

            case SketchMode.MotionEdit:
                if (_charModel.SelectedJointIndex != -1)
                {
                        
                    if (points.Count < _sketchModel.GetPrevFrame() + _sketchModel.GetFolFrame())
                    {
                        Debug.Log("Sketch Trajectory is too short!!!");
                        break;
                    }
                    _sketchModel.sketchTrajectory2D = points;
                    SketchimoCore.Instance.TrajectoryEdit();
                }
                break;

            case SketchMode.VelocityEdit:
// 		if (CharacterModel->SelectedJointIndex != -1)
// 		{
// 			SketchModel->SketchTrajectory2D = Points;
// 			UVMSketchimoCore::SelectSubTrajectoryBySketch(Points);
// 			if (MotionModel->SubTrajectoryIndex.Num() == 0)
// 			{
// 				VMLog(FString::Printf(TEXT("Selected editing trajectory is too short!")));
// 				return;
// 			}
//
// 			UVMSketchimoCore::EditVelocity();
// 		}
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // update 3D Trajectory after draw input finished
        SketchimoCore.Instance.RetrieveTrajectory3D();

        // Update motion after editing 
        _charModel.SetPose(_motionModel.GetCurrentPose());
    }

    public void SetMode(SketchMode mode)
    {
        _sketchModel.SetCurrentMode(mode);

        switch (mode)
        {
            case SketchMode.Select:
                // _charModel.selectedBone = default;
                // _sketchModel.ResetSketchTrajectory();
                break;
            case SketchMode.BodyLineEdit:
                // UVMSketchimoCore::RetrieveTrajectory3D();
                break;
            case SketchMode.MotionEdit:
                // UVMSketchimoCore::RetrieveTrajectory3D();
                break;
            default:
                break;
        }
    }
}
}