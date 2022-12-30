using System.Collections.Generic;
using Sketchimo.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sketchimo.Views
{
public class SketchInput : MonoBehaviour
{
    public Camera cam;

    public GameObject brush;

    public SketchController sketchController;

    private void Start()
    {
        sketchController = GetComponent<SketchController>();
    }

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            CreateBrush();
            sketchController.SetDrawInput(_positions);
        }
        else if (Input.GetKey(KeyCode.Mouse0) && _currentLineRenderer != default)
        {
            AddAPoint();
            sketchController.SetDrawInput(_positions);
        }
        else
        {
            if (_currentLineRenderer == default) return;

            sketchController.SetDrawInputFinished(_positions);
            DestroyBrush();
        }
    }

    private void CreateBrush()
    {
        var brushInterface = Instantiate(brush);
        _currentLineRenderer = brushInterface.GetComponent<LineRenderer>();
        var mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));

        _currentLineRenderer.SetPosition(0, mousePos);
        _currentLineRenderer.SetPosition(1, mousePos);

        _lastPos = mousePos;

        _positions.Add(Input.mousePosition);
    }

    private void AddAPoint()
    {
        var mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
        if (mousePos == _lastPos) return;

        var positionIndex = _currentLineRenderer.positionCount++;
        _currentLineRenderer.SetPosition(positionIndex, mousePos);

        _lastPos = mousePos;
        _positions.Add(Input.mousePosition);
    }

    private void DestroyBrush()
    {
        Destroy(_currentLineRenderer.gameObject);
        _currentLineRenderer = default;
        _positions.Clear();
    }

    private readonly List<Vector2> _positions = new List<Vector2>();

    private LineRenderer _currentLineRenderer;

    private Vector3 _lastPos;
}
}