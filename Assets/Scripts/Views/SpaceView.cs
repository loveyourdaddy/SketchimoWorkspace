using Sketchimo.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sketchimo.Views
{
public class SpaceView : MonoBehaviour
{
    public Button global;
    public Button local;
    public Button dynamic;

    public SketchInfo sketchModel;

    private void Start()
    {
        global.onClick.AddListener(() => sketchModel.SetCurrentSpace(SketchSpace.Global));
        local.onClick.AddListener(() => sketchModel.SetCurrentSpace(SketchSpace.Local));
        dynamic.onClick.AddListener(() => sketchModel.SetCurrentSpace(SketchSpace.Dynamic));
    }
}
}