using Sketchimo.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sketchimo.Views
{
public class EditModeView : MonoBehaviour
{
    public Button select;
    public Button bodyLineEdit;
    public Button motionEdit;
    public Button velocityEdit;

    public SketchInfo sketchModel;

    private void Start()
    {
        select.onClick.AddListener(() => sketchModel.SetCurrentMode(SketchMode.Select));
        bodyLineEdit.onClick.AddListener(() => sketchModel.SetCurrentMode(SketchMode.BodyLineEdit));
        motionEdit.onClick.AddListener(() => sketchModel.SetCurrentMode(SketchMode.MotionEdit));
        velocityEdit.onClick.AddListener(() => sketchModel.SetCurrentMode(SketchMode.VelocityEdit));
    }
}
}