using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Sketchimo.Models
{
public class CharInfo : MonoBehaviour
{
    public List<Vector3> boneOffset;
    public Action OnCharacterChanged;
    public Action OnSelectedChainChanged;

    public int SelectedJointIndex => GetIndexOf(selectedBone);
    public GameObject startBone, endBone, selectedBone;

    public GameObject currentActor;

    public GameObject GetMesh()
    {
        return _mesh;
    }

    public void SetMesh(GameObject mesh)
    {
        LoadMeshData(mesh);
        OnCharacterChanged?.Invoke();
        SetNumOfChild();

        // todo : 캐릭터 바꿀 시, BoneOffset 수정.
    }

    public GameObject[] GetBones()
    {
        return _bones;
    }

    public GameObject GetBoneAt(int index)
    {
        return _bones[index];
    }

    public List<GameObject> GetSelectedChain()
    {
        return _selectedChain;
    }

    public List<int> GetSelectedChainIdxes()
    {
        return _selectedChain.Select(GetIndexOf).ToList();
    }

    public void SetSelectedChain(List<GameObject> selectedChain)
    {
        _selectedChain = selectedChain;
        OnSelectedChainChanged?.Invoke();
    }

    public void SetPath(GameObject from, GameObject to)
    {
        if (from == default || to == default) return;

        if (startBone == from && endBone == to) return;

        startBone = from;
        endBone = to;
        selectedBone = endBone;

        SetSelectedChain(SketchimoCore.Instance.GetChainFromStartToEnd(startBone, endBone));
    }

    public void SetBoneOffset()
    {
        boneOffset.Clear();
        boneOffset.Capacity = _bones.Length;
        foreach (var bone in _bones)
            boneOffset.Add(bone.transform.localPosition);
    }

    public int GetNumOfChild(GameObject bone)
    {
        if (_numOfChild.ContainsKey(bone))
        {
            return _numOfChild[bone];
        }

        return 0;
    }

    public int GetIndexOf(GameObject bone)
    {
        if (!_indexOfBone.ContainsKey(bone))
        {
            Debug.LogError($"Can't find {bone.name}");
            return -1;
        }

        return _indexOfBone[bone];
    }

    private void LoadMeshData(GameObject mesh)
    {
        _mesh = mesh;
        _bones = _mesh.GetComponentsInChildren<Transform>().Select(p => p.gameObject).ToArray();

        _indexOfBone.Clear();
        for (var i = 0; i < _bones.Length; i++)
            _indexOfBone[_bones[i]] = i;

        SetBoneOffset();
    }

    private void SetNumOfChild()
    {
        _numOfChild.Clear();

        for (var i = 1; i < _bones.Length; i++)
        {
            var parent = _bones[i].transform.parent.gameObject;

            if (_numOfChild.ContainsKey(parent))
            {
                ++_numOfChild[parent];
            }
            else
            {
                _numOfChild[parent] = 1;
            }
        }
    }

    public void SetPose(PoseData pose)
    {
        _bones[0].transform.position = pose.joints[0].position;
        _bones[0].transform.rotation = pose.joints[0].rotation;

        for (var i = 1; i < _bones.Length; i++)
        {
            var bone = _bones[i];
            var newRoot = pose.joints[0];
            var newBone = pose.joints[i];

            bone.transform.position = newRoot.position + newRoot.rotation * newBone.position;
            bone.transform.rotation = newRoot.rotation * newBone.rotation;
        }
    }

    private void PrintLog()
    {
        print("Character Changed!");
    }

    private GameObject[] _bones;
    private GameObject _mesh; // TODO: Change name to pelvis
    private List<GameObject> _selectedChain = new List<GameObject>();

    private readonly Dictionary<GameObject, int> _indexOfBone = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> _numOfChild = new Dictionary<GameObject, int>();
}
}