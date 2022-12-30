using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sketchimo.Models;
using UnityEngine.UI;
using Sketchimo.Views;
using Sketchimo.Controllers;
namespace Sketchimo.Views
{ 
public class LengthController : MonoBehaviour
{
	public Slider left;
	public Slider right;
	public SliderView sliderView;
	public MotionInfo motionModel;
	public MotionController motionController;

	private bool right_pointer_down = false;
	private bool left_pointer_down = false;

	private SketchInfo sketchInfo;

	private void Start()
	{
		sketchInfo = FindObjectOfType<SketchInfo>();
		StartCoroutine(Start_Coroutine());
	}

	void Update()
    {
		if (!left_pointer_down)
			left.value = sliderView.slider.value - sketchInfo.GetPrevFrame() > 0 ? sliderView.slider.value - sketchInfo.GetPrevFrame() : 0;
		if (!right_pointer_down)
			right.value = sliderView.slider.value + sketchInfo.GetFolFrame() < motionModel.GetTotalFrame() ? sliderView.slider.value + sketchInfo.GetFolFrame() : motionModel.GetTotalFrame();
	}

	public IEnumerator Start_Coroutine()
	{
		yield return new WaitUntil(() => motionModel.GetTotalFrame() != 0);

		left.maxValue = motionModel.GetTotalFrame();
		right.maxValue = motionModel.GetTotalFrame();
	}

	public void SetFolFrame()
	{
		if(right_pointer_down)
			sketchInfo.SetFolFrame((int)(right.value - sliderView.slider.value));
	}

	public void SetPrevFrame()
	{
		if (left_pointer_down)
			sketchInfo.SetPrevFrame((int)(sliderView.slider.value - left.value));
	}

	public void RightPointerDown()
	{
		right_pointer_down = true;
	}

	public void RightPointerUp()
	{
		right_pointer_down = false;
	}

	public void LeftPointerDown()
	{
		left_pointer_down = true;
	}

	public void LeftPointerUp()
	{
		left_pointer_down = false;
	}
}
}