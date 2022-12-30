using System.Collections;
using System.Collections.Generic;
using Sketchimo.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Sketchimo.Views
{
public class PlayButtonView : MonoBehaviour
{
    public Button[] buttons;
    private MotionController motionController;

	private void Start()
	{
        motionController = FindObjectOfType<MotionController>();
        motionController.OnPlayStateChanged += OnStateChanged;
	}

	// Start is called before the first frame update
	public void OnStateChanged()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].gameObject.SetActive(!((int)motionController.CurrentPlay == i));
    }
}
}
