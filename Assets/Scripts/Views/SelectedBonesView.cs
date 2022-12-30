using Sketchimo.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sketchimo.Views
{
public class SelectedBonesView : MonoBehaviour
{
    public GameObject brushPrefab;

    public Material selectedBonesMat;

    public CharInfo charModel;

    // Start is called before the first frame update
    private void Start()
    {
        _textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        var selectedBones = charModel.GetSelectedChain();
        if (selectedBones.Count == 0)
        {
            _textComp.text = "";
            _selectedBonesRenderer = default;
            return;
        }

        // Update Text
        {
            var text = selectedBones[0].name;

            for (var i = 1; i < selectedBones.Count; i++)
            {
                var bone = selectedBones[i];
                text += $" - {bone.name}";
            }

            _textComp.text = text;
        }

        // Draw Lines
        {
            if (_selectedBonesRenderer == default)
            {
                _selectedBonesRenderer = Instantiate(brushPrefab).GetComponent<LineRenderer>();
                _selectedBonesRenderer.material = selectedBonesMat;
                _selectedBonesRenderer.material.renderQueue = 3000;
            }

            var trajectoryPos = new Vector3[selectedBones.Count];
            for (var i = 0; i < selectedBones.Count; i++)
            {
                var bone = selectedBones[i];
                trajectoryPos[i] = bone.transform.position;
            }

            _selectedBonesRenderer.positionCount = trajectoryPos.Length;
            _selectedBonesRenderer.SetPositions(trajectoryPos);
        }
    }

    private Text _textComp;
    private LineRenderer _selectedBonesRenderer;
}
}