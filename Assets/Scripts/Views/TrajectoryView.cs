using Sketchimo.Models;
using UnityEngine;

namespace Sketchimo.Views
{
public class TrajectoryView : MonoBehaviour
{
    public GameObject brushPrefab;

    public Material trajectory3DMat;

    private void Start()
    {
        _charModel = GetComponent<CharInfo>();
        _motionModel = GetComponent<MotionInfo>();
    }

    private void Update()
    {
        var selectedBone = _charModel.selectedBone;
        if (selectedBone == default) return;
            SketchimoCore.Instance.RetrieveTrajectory3D();

        var selectedBoneIndex = _charModel.GetIndexOf(selectedBone);
        var poses = _motionModel.motion.data;
        var startFrame = _motionModel.trajectoryStartFrame;
        var endFrame = _motionModel.trajectoryEndFrame;

        if (endFrame <= startFrame + 1)
        {
            return;
        }

        if (_trajectory3DRenderer == default)
        {
            _trajectory3DRenderer = Instantiate(brushPrefab).GetComponent<LineRenderer>();
            _trajectory3DRenderer.material = trajectory3DMat;
        }

        var trajectoryPos = new Vector3[endFrame - startFrame];
        for (int i = startFrame; i < endFrame; ++i)
        {
            var framePos = _motionModel.trajectory3D[i - startFrame];
            trajectoryPos[i - startFrame] = framePos;
        }

        _trajectory3DRenderer.positionCount = trajectoryPos.Length;
        _trajectory3DRenderer.startWidth = 0.01f;
        _trajectory3DRenderer.endWidth = 0.03f;
        _trajectory3DRenderer.SetPositions(trajectoryPos);
    }

    private CharInfo _charModel;
    private MotionInfo _motionModel;
    private LineRenderer _trajectory3DRenderer;
}
}