using System.Collections.Generic;
using System.Linq;
using Sketchimo.Models;
using UnityEngine;
using Utils;

namespace Sketchimo
{
public class SketchimoCore : MonoBehaviour
{
    public static SketchimoCore Instance;
    public CameraInfo cameraModel;
    public CharInfo charModel;
    public MotionInfo motionModel;
    public SketchInfo sketchModel;
    public MovementLimitInfo movementLimitInfo;

    public void Awake()
    {
        Instance = this;
    }

    public void SelectBones(List<Vector2> points)
    {
        var startBone = GetClosestBone(points[0]);
        var endBone = GetClosestBone(points.Last());
        charModel.SetPath(startBone, endBone);
    }

    public void BodyLineEdit()
    {
        RetrieveTrajectory3D();

        RefineSketchTrajectory2D(charModel.GetSelectedChain().Count);
        DeprojectSketchTrajectory2Dto3D();

        if (sketchModel.GetSketchTrajectory3DNum() < charModel.GetSelectedChain().Count)
        {
            return;
        }

        ApplySketchTrajectoryToBodyLine();
    }

    public void TrajectoryEdit()
    {
        RetrieveTrajectory3D();

        var TrajectoryNum = motionModel.trajectory3D.Count;
        if (TrajectoryNum == 0) return;

        RefineSketchTrajectory2D(TrajectoryNum);
        DeprojectSketchTrajectory2Dto3D();

        var TargetTrajectory = GetTargetTrajectory();
        if (TargetTrajectory.Count == 0) return;

        ApplyTargetTrajectoryToMotion(TargetTrajectory);
    }

    public void RefineSketchTrajectory2D(int trajectoryNum)
    {
        var sketchTrajectory2DNum = sketchModel.GetSketchTrajectory2DNum();

        // interpolate sketch trajectory
        var interval = sketchTrajectory2DNum / trajectoryNum;

        Vector2[] interpolatedSketchTrajectory2D = new Vector2[trajectoryNum];

        for (int i = 0; i < trajectoryNum; i++)
            interpolatedSketchTrajectory2D[i] = sketchModel.sketchTrajectory2D[i * interval];

        sketchModel.sketchTrajectory2D = interpolatedSketchTrajectory2D.ToList();
    }


    public void DeprojectSketchTrajectory2Dto3D()
    {
        var selectedJointPosition = charModel.selectedBone.transform.position;

        var depth = Vector3.Distance(selectedJointPosition, cameraModel.cam.transform.position);


        var sketchTrajectory3D = new Vector3[sketchModel.GetSketchTrajectory2DNum()];

        for (int i = 0; i < sketchTrajectory3D.Length; i++)
        {
            var screenPoint = sketchModel.sketchTrajectory2D[i];
            var screenPointWithDepth = new Vector3(screenPoint.x, screenPoint.y, depth);

            sketchTrajectory3D[i] = cameraModel.cam.ScreenToWorldPoint(screenPointWithDepth);
        }

        sketchModel.sketchTrajectory3D = sketchTrajectory3D.ToList();
    }

    public void RetrieveTrajectory3D()
    {
        var JointIdx = charModel.SelectedJointIndex;
        if (JointIdx < 0)
        {
            return;
        }

        motionModel.trajectory3D = RetrieveTrajectory3D(JointIdx);
    }

    public List<Vector3> RetrieveTrajectory3D(int JointIdx)
    {
        var StartFrame = motionModel.trajectoryStartFrame;
        var EndFrame = motionModel.trajectoryEndFrame;

        var trajectory = new List<Vector3>();

        var Space = sketchModel.GetCurrentSpace();
        switch (Space)
        {
            case SketchSpace.Global:
                for (var i = StartFrame; i < EndFrame; i++)
                    trajectory.Add(motionModel.motion.data[i].GetWorldPositionOf(JointIdx));
                break;
            case SketchSpace.Local:
                {
                    var joint = charModel.GetBoneAt(JointIdx);
                    var CurrentPose = motionModel.GetCurrentPose();
                    var parentBone = joint.transform.parent.gameObject;
                    if (parentBone == default)
                    {
                        Debug.LogError("Can't edit this joint");
                        break;
                    }
                    var parentBoneIdx = charModel.GetIndexOf(parentBone);
                    var grandParentBone = parentBone.transform.parent.gameObject;
                    if (grandParentBone == default)
                    {
                        Debug.LogError("Can't edit this joint");
                        break;
                    }
                    var grandParentBoneIdx = charModel.GetIndexOf(grandParentBone);
                    var currentParentWorldPosition = CurrentPose.GetWorldPositionOf(parentBoneIdx);
                    var curretnGrandParentWorldRotation = CurrentPose.GetWorldRotationOf(grandParentBoneIdx);

                    for (int i = StartFrame; i < EndFrame; i++)
                    {
                        var selectedJointWorldPosition = motionModel.motion.data[i].GetWorldPositionOf(JointIdx);
                        var parentWorldPosition = motionModel.motion.data[i].GetWorldPositionOf(parentBoneIdx);
                        var grandParentWorldRotation = motionModel.motion.data[i].GetWorldRotationOf(grandParentBoneIdx);
                        var grandParentWorldRotationInverse = Quaternion.Inverse(grandParentWorldRotation);
                        var localPosition = grandParentWorldRotationInverse * (selectedJointWorldPosition - parentWorldPosition);
                        var currentWorldPosition = curretnGrandParentWorldRotation * localPosition + currentParentWorldPosition;

                        trajectory.Add(currentWorldPosition);
                    }
                }
                break;
            case SketchSpace.Dynamic:
                {
                    List<Vector3> WorldTrajectory = new List<Vector3>();
                    for (int i = StartFrame; i < EndFrame; i++)
                    {
                        WorldTrajectory.Add(motionModel.GetPoseAt(i).GetWorldPositionOf(JointIdx));
                    }
                    
                    var DynamicWarpingVector = GetDynamicWarpingVector(WorldTrajectory);
                    int len = WorldTrajectory.Count;
                    for (int i = 0; i < len; i++)
                    {
                        trajectory.Add(WorldTrajectory[i] + DynamicWarpingVector[i]);
                    }
                }
                break;
        }

        return trajectory;
    }

    List<Vector3> GetDynamicWarpingVector(List<Vector3> WorldTrajectory)
    {
        // Get Current Position 
        int StartFrame = motionModel.trajectoryStartFrame;
        int CurrentFrame = motionModel.GetCurrentFrame();
        int EndFrame = motionModel.trajectoryEndFrame;

        int CurrentIdxinTrajectory;
        if (CurrentFrame > sketchModel.GetPrevFrame()) // >10
        {
            CurrentIdxinTrajectory = sketchModel.GetPrevFrame();
        }
        else // <=10
        {
            CurrentIdxinTrajectory = CurrentFrame;
        }

        // Get Camera Info
        Vector3 CameraPosition = cameraModel.CameraLocation;
        Vector3 CurrentRootPosition = motionModel.GetPoseAt(0).GetWorldPositionOf(0);
        Vector3 CameraDirection = (CameraPosition - CurrentRootPosition).normalized;

        // Get Warping vector and return it
        List<Vector3> DynamicWarpingVector = new List<Vector3>();
        int len = WorldTrajectory.Count;

        for (int i = 0, FrameIndex = StartFrame; i < len; i++, ++FrameIndex)
        {
            // Projection to plane
            Vector3 PositionDisp = WorldTrajectory[CurrentIdxinTrajectory] - WorldTrajectory[i];
            float DepthAxisDisp = Vector3.Dot(PositionDisp, CameraDirection);

            // Warping in time-axis 
            int FrameDisp = (CurrentIdxinTrajectory - i);
            var UpVector = cameraModel.CameraUpVector;

            Vector3 TimeAxis = Vector3.Cross(UpVector, CameraDirection);
            Vector3 TimeAxisDisp = FrameDisp * TimeAxis;

            var WarpingVector = DepthAxisDisp * CameraDirection + sketchModel.GetAlpha() * TimeAxisDisp;
            DynamicWarpingVector.Add(WarpingVector);
        }

        return DynamicWarpingVector;
    }

    List<Vector3> GetTargetTrajectory()
    {
        var TrajectorySize = motionModel.trajectory3D.Count;
        var TrajectoryMid = motionModel.GetCurrentFrame() - motionModel.trajectoryStartFrame;
        List<float> Weights = GetWeightDistribution(TrajectorySize, TrajectoryMid);


        var SketchTrajectory2DNum = sketchModel.GetSketchTrajectory2DNum();
        if (SketchTrajectory2DNum != TrajectorySize)
        {
            Debug.Log("Sketch length is shorter than trajectory length: " + SketchTrajectory2DNum);
            // todo : Sketch Should be extrapolated 
            return new List<Vector3>();
        }

        var SketchTrajectory = sketchModel.sketchTrajectory3D;
        var MotionTrajectory = motionModel.trajectory3D;

        var FirstFrameDisp = MotionTrajectory[0] - SketchTrajectory[0];
        var LastFrameDisp = MotionTrajectory[TrajectorySize - 1] - SketchTrajectory[TrajectorySize - 1];

        List<Vector3> TargetTrajectory = new List<Vector3>(new Vector3[TrajectorySize]);
        for (int i = 0; i < TrajectoryMid; ++i)
        {
            TargetTrajectory[i] = SketchTrajectory[i] + (1 - Weights[i]) * FirstFrameDisp;
        }

        for (int i = TrajectoryMid; i < TrajectorySize; ++i)
        {
            TargetTrajectory[i] = SketchTrajectory[i] + (1 - Weights[i]) * LastFrameDisp;
        }

        return TargetTrajectory;
    }

    List<float> GetWeightDistribution(int Size, int Mid)
    {
        List<float> Weights = new List<float>(new float[Size]);
        //Weights = new List<float>(new float[Size]);

        float Interval = 1.0f / Mid;
	    for (int i = 0; i<Mid; ++i)
	    {
		    Weights[i] = i* Interval;
        }
        Interval = 1.0f / (Size - 1 - Mid);
	    for (int i = Mid; i<Size; ++i)
	    {
		    Weights[i] = (Size - 1 - i) * Interval;
        }
        return Weights;
    }

    public void ApplySketchTrajectoryToBodyLine()
    {
        var mesh = charModel.GetMesh();
        var startFrame = motionModel.trajectoryStartFrame;
        var currentFrame = motionModel.GetCurrentFrame();
        var endFrame = motionModel.trajectoryEndFrame;
        var currentTrajectoryIndex = currentFrame - startFrame;
        var remainTrajectoryIndex = endFrame - currentFrame;
        var sketchTrajectory = sketchModel.sketchTrajectory3D;
        var selectedJointsIdxes = charModel.GetSelectedChainIdxes();
        var selectedJointsNum = selectedJointsIdxes.Count;

        var frontRootJointIdx = 0;
        var backRootJointIdx = selectedJointsNum - 1;

        var currentPose = motionModel.motion.data[currentFrame];
        var originalPose = new PoseData(currentPose);

        //find out Front root joint
        for (var selectedJointIdx = 0; selectedJointIdx < selectedJointsNum; ++selectedJointIdx)
        {
            var selectedBone = charModel.GetBoneAt(selectedJointsIdxes[selectedJointIdx]);
            if (selectedBone.transform.name == "mixamorig:RightHand" || selectedBone.transform.name == "mixamorig:LeftHand" ||
                    selectedBone.transform.name == "left_wrist" || selectedBone.transform.name == "right_wrist")
                continue;
            if (charModel.GetNumOfChild(selectedBone) > 1)
            {
                frontRootJointIdx = selectedJointIdx - 1;
                break;
            }
        }

        //find out Back root joint
        for (var selectedJointIdx = selectedJointsNum - 1; selectedJointIdx >= 0; --selectedJointIdx)
        {
            var selectedBoneName = charModel.GetBoneAt(selectedJointsIdxes[selectedJointIdx]);
            if (selectedBoneName.transform.name == "mixamorig:RightHand" || selectedBoneName.transform.name == "mixamorig:LeftHand" ||
                        selectedBoneName.transform.name == "left_wrist" || selectedBoneName.transform.name == "right_wrist")
                continue;
            if (charModel.GetNumOfChild(selectedBoneName) > 1)
            {
                backRootJointIdx = selectedJointIdx + 1;
                break;
            }
        }

        // Set the smallest index to root if hadn't find any root.
        if (frontRootJointIdx == 0 && backRootJointIdx == selectedJointsNum - 1)
        {
            frontRootJointIdx = selectedJointsIdxes[0] < selectedJointsIdxes.Last() ? 0 : selectedJointsIdxes.Count - 1;
            backRootJointIdx = frontRootJointIdx;
        }

        var frontRootIdx = selectedJointsIdxes[frontRootJointIdx];
        var backRootIdx = selectedJointsIdxes[backRootJointIdx];

        // Change current pose
        for (var jointIdx = frontRootJointIdx - 1; jointIdx > 0; --jointIdx) // =
            {
            var selectedJointIdx = selectedJointsIdxes[jointIdx];
            var targetWorldPosition = sketchTrajectory[jointIdx];
            InverseKinematics(currentFrame, frontRootIdx, selectedJointIdx, targetWorldPosition);
        }

        // TODO: 카메라 to screen depth 때문에 역방향 ik가 이상하게 풀리고 있음
        for (var jointIdx = backRootJointIdx + 1; jointIdx < selectedJointsNum; ++jointIdx)
        {
            var selectedJointIdx = selectedJointsIdxes[jointIdx];
            var targetWorldPosition = sketchTrajectory[jointIdx];
            InverseKinematics(currentFrame, backRootIdx, selectedJointIdx, targetWorldPosition);
        }

        charModel.SetPose(currentPose);

        // Interpolation through joints
        for (var i = 0; i < selectedJointsNum; ++i)
        {
            var jointIdx = selectedJointsIdxes[i];
            var originalRotation = originalPose.GetLocalRotationOf(jointIdx);
            var newRotation = currentPose.GetLocalRotationOf(jointIdx);
            var diff = newRotation * Quaternion.Inverse(originalRotation);

            if (diff.w > 1f - Quaternion.kEpsilon)
            {
                continue;
            }

            // Interpolation through frames
            for (var frameIndex = startFrame + 1; frameIndex < endFrame; ++frameIndex)
            {
                if (frameIndex == currentFrame)
                {
                    continue;
                }

                // TODO: Ease in ease out
                var slerpRatio = Mathf.Abs(frameIndex - currentFrame) / (float) currentTrajectoryIndex;
                slerpRatio = 1f - slerpRatio * slerpRatio;

                var deltaRotation = Quaternion.Slerp(Quaternion.identity, diff, slerpRatio);
                motionModel.motion.data[frameIndex].RotateJointLocal(jointIdx, deltaRotation);
            }
        }
    }

    void ApplyTargetTrajectoryToMotion(List<Vector3> TargetTrajectory)
    {
        // i : StartFrame ~ EndFrame
        // TrajectoryIndex : 0 ~ TrajectorySize
        var StartFrame = motionModel.trajectoryStartFrame;
        var EndFrame = motionModel.trajectoryEndFrame;
        var JointIndex = charModel.SelectedJointIndex;

        switch (sketchModel.GetCurrentSpace())
        {
            case SketchSpace.Global:
                for (int i = StartFrame, TrajectoryIndex = 0; i < EndFrame; i++, TrajectoryIndex++)
                {
                    ApplyTargetPositionToPose(i, JointIndex, TargetTrajectory[TrajectoryIndex]);
                }
                break;
            case SketchSpace.Local:
                {
                    var bone = charModel.GetBoneAt(JointIndex);
                    var parentBone = bone.transform.parent.gameObject;
                    var parentBoneIndex = charModel.GetIndexOf(parentBone);
                    var CurrentPose = motionModel.GetCurrentPose();

                    var currentJointWorldPosition = CurrentPose.GetWorldPositionOf(JointIndex);
                    var currentParentWorldPosition = CurrentPose.GetWorldPositionOf(parentBoneIndex);
                    var currentDir = currentJointWorldPosition - currentParentWorldPosition;

                    for (int i = StartFrame, TrajectoryIndex = 0; i < EndFrame; i++, TrajectoryIndex++)
                    {
                        // TODO : Check if the TargetTrajectory list contains local positions -> Should be converted to world positions 
                        var TargetPosition = GetWorldPosition(TargetTrajectory[TrajectoryIndex]);
                        var TargetDir = TargetPosition - currentParentWorldPosition;

                        var angle = Vector3.Angle(currentDir, TargetDir);
                        var rotationAxis = Vector3.Cross(currentDir, TargetDir);
                        var deltaRotation = Quaternion.AngleAxis(angle, rotationAxis);

                        var pose = motionModel.GetPoseAt(i);
                        var ParentRotation = pose.GetLocalRotationOf(parentBoneIndex);
                        pose.RotateJointLocal(parentBoneIndex, deltaRotation);

                        charModel.SetPose(pose);
                    }
                }
                break;
            case SketchSpace.Dynamic:
                {
                    // Current Frame & mid of dynamic trajectory
                    int CurrentFrame = motionModel.GetCurrentFrame();
                    int CurrentIdxinTrajectory;
                    if (CurrentFrame > sketchModel.GetPrevFrame())
                    {
                        CurrentIdxinTrajectory = sketchModel.GetPrevFrame();
                    }
                    else // <=10
                    {
                        CurrentIdxinTrajectory = CurrentFrame;
                    }

                    // original pose & Camera direction 
                    Vector3 CameraPosition = cameraModel.CameraLocation;
                    Vector3 CurrentRootPosition = motionModel.GetPoseAt(0).GetWorldPositionOf(0);
                    Vector3 CameraDirection = (CameraPosition - CurrentRootPosition).normalized;

                    for (int i = StartFrame, TrajectoryIndex = 0; i < EndFrame; i++, TrajectoryIndex++)
                    {
                       Vector3 DynamicTargetPosition = TargetTrajectory[TrajectoryIndex];

                       // Warping in time-axis 
                       float FrameDisp = (float)(CurrentIdxinTrajectory - TrajectoryIndex);
                       Vector3 UpVector = cameraModel.CameraUpVector;
                       Vector3 TimeAxis = Vector3.Cross(UpVector, CameraDirection);
                       Vector3 TimeAxisDisp = FrameDisp * TimeAxis;

                       // Deprojection in camera direction 
                       Vector3 PositionDisp = motionModel.GetPoseAt(CurrentFrame).GetWorldPositionOf(JointIndex) - motionModel.GetPoseAt(i).GetWorldPositionOf(JointIndex);
                       Vector3 Deprojection = Vector3.Dot(PositionDisp, CameraDirection) * CameraDirection;

                       // Get warping vector 
                       Vector3 DynamicWarping = sketchModel.GetAlpha() * TimeAxisDisp + Deprojection;
                       print(DynamicWarping);

                       // Get Target position in global editing space
                       Vector3 TargetPosition = DynamicTargetPosition - DynamicWarping;

                       // solving IK for joint to reach to target position
                       ApplyTargetPositionToPose(i, JointIndex, TargetPosition);
                    }
                }
                break;

            default:
                break;
        }

        // For Debug
        motionModel.targetTrajectory3D = TargetTrajectory;
    }

    Vector3 GetWorldPosition(Vector3 WorldPositionInActor)
    {
        var currentFrame = motionModel.GetCurrentFrame();
        var currentPose = motionModel.motion.data[currentFrame];

        var RootRotation = currentPose.joints[0].rotation;
        var RootPosition = currentPose.joints[0].position;

        var WorldPosition = Quaternion.Inverse(RootRotation) * (WorldPositionInActor - RootPosition);

        return WorldPosition;
    }

    void ApplyTargetPositionToPose(int Frame, int JointIndex, Vector3 TargetPosition)
    {
        var pose = motionModel.motion.data[Frame];

        if (JointIndex == 0)
        {
            pose.joints[JointIndex].position = TargetPosition;
        }
        else
        {
            //update local orientation to reach the target position 
            InverseKinematics(Frame, JointIndex, TargetPosition, pose);
        }
        //var jointWorldPositions = ForwardKinematics(pose.joints[0].position, pose.GetLocalRotations(), pose);
        //var count = jointWorldPositions.Count;
        ////var rootQuatInverse = pose.joints[0].rotation; rootQuatInverse * 
        //for(int i=0; i<count; i++)
        //{
        //    pose.joints[i].position = (jointWorldPositions[i] - pose.joints[0].position);
        //}
    }

    public List<Vector3> ForwardKinematics(Vector3 RootPos, List<Quaternion> LocalOrientation, PoseData pose)
    {
        int len = LocalOrientation.Count;
        List<Vector3> WorldPositions = new List<Vector3>(new Vector3[len]);

        for (int JointIndex = 0; JointIndex < len; JointIndex++)
        {
            if (JointIndex == 0)
            {
                WorldPositions[JointIndex] = RootPos; // Vector3.zero; 
                continue;
            }
            int parentIdx = pose.joints[JointIndex].parentIdx;

            WorldPositions[JointIndex] = WorldPositions[parentIdx]
                    + pose.GetWorldRotationOf(parentIdx) * charModel.boneOffset[JointIndex];
        }
        return WorldPositions;
    }

    void InverseKinematics(int Frame, int JointIndex, Vector3 TargetPosition, PoseData pose)
    {
        // TODO: Set root index to chest when the joint is on arms or head.
        // reach the division joint(rootBoneIndex)
        var currentJoint = JointIndex;
        while (pose.joints[currentJoint].childrenIdx.Count < 2) 
        {
            var parentJoint = pose.joints[currentJoint].parentIdx;
            if (parentJoint == 0)
                break;
             currentJoint = parentJoint;
        }

        var RootBoneIndex = currentJoint;
        // IK from division joint to SelectionJoint
        InverseKinematics(Frame, RootBoneIndex, JointIndex, TargetPosition);
    }

    public void InverseKinematics(int Frame, int RootIndex, int JointIndex, Vector3 TargetPosition)
    {
        var pose = motionModel.GetPoseAt(Frame);
        var currentPositions = pose.GetWorldPositions();

        // CCD 
        var ikChain = new List<GameObject>();
        var bone = charModel.GetBoneAt(JointIndex);
        var rootBone = charModel.GetBoneAt(RootIndex);
        while (bone != rootBone)
        {
            ikChain.Add(bone);
            bone = bone.transform.parent.gameObject;
        }

        if (bone == default)
        {
            Debug.LogWarning("TipBoneName is NOT a child of RootBoneName");
            return;
        }

        // solve
        const float precision = 0.01f;
        const int maxIterations = 50;
        var iterationCount = 0;
        var distance = Vector3.Distance(currentPositions[JointIndex], TargetPosition);

        while (distance > precision && iterationCount++ < maxIterations)
        {
            foreach (var currentBone in ikChain)
            {
                var eePosition = currentPositions[JointIndex];
                var parentBoneIndex = charModel.GetIndexOf(currentBone.transform.parent.gameObject);

                var parentBonePosition = currentPositions[parentBoneIndex];
                var toEE = (eePosition - parentBonePosition).normalized;
                var toTarget = (TargetPosition - parentBonePosition).normalized;
                if (toEE == Vector3.zero || toTarget == Vector3.zero)
                {
                    continue;
                }

                var angle = Vector3.Angle(toEE, toTarget) / 10; // todo : (issue) ccd solve oscillation
                var rotationAxis = Vector3.Cross(toEE, toTarget);

                var deltaRotation = Quaternion.AngleAxis(angle, rotationAxis);

                pose.RotateJoint(parentBoneIndex, deltaRotation);
                pose.ClampJoint(parentBoneIndex, movementLimitInfo);    
                currentPositions = pose.GetWorldPositions();
            }

            distance = Vector3.Distance(currentPositions[JointIndex], TargetPosition);
        }

        // motionModel.motion.data[Frame] = pose;
    }

    private GameObject GetClosestBone(Vector2 point)
    {
        var ray = cameraModel.cam.ScreenPointToRay(point);

        var mesh = charModel.GetMesh();
        var joints = mesh.GetComponentsInChildren<Transform>();
        var boneNum = joints.Length;

        float maxDistance = 50f;
        var closestDistance = maxDistance;
        var closestBone = mesh;

        for (var i = 0; i < boneNum; ++i)
        {
            var bone = joints[i];
            var boneLocation = bone.position;
            var distance = Vector3.Cross(ray.direction, boneLocation - ray.origin).magnitude;

            if (closestDistance > distance)
            {
                closestDistance = distance;
                closestBone = bone.gameObject;
            }
        }

        return closestBone;
    }


    public List<GameObject> GetChainFromStartToEnd(GameObject startBone, GameObject endBone)
    {
        if (startBone == endBone)
        {
            return new List<GameObject> {startBone};
        }

        var startChain = GetChainToRoot(startBone);
        var endChain = GetChainToRoot(endBone);

        int duplicatedIndex = -1;

        for (var i = 0;; ++i)
            if (i == startChain.Count || i == endChain.Count || startChain[i] != endChain[i])
            {
                duplicatedIndex = i - 1;
                break;
            }

        if (duplicatedIndex < 0)
        {
            Debug.LogError("Can not find the duplicated joint parent");
            return startChain;
        }

        var startToEndChain = startChain.GetRange(duplicatedIndex, startChain.Count - duplicatedIndex);
        startToEndChain.Reverse();

        startToEndChain.AddRange(endChain.GetRange(duplicatedIndex + 1, endChain.Count - duplicatedIndex - 1));

        return startToEndChain;
    }


    private List<GameObject> GetChainToRoot(GameObject bone)
    {
        var chain = new List<GameObject> {bone};

        while (true)
        {
            if (bone.name.ToLower().Contains("pelvis")) break;
            bone = bone.transform.parent.gameObject;
            chain.Add(bone);
        }

        chain.Reverse(); // Root to bone;
        return chain;
    }
}
}