using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Sketchimo.Models
{
public class SelectionInfo_Animation : SelectionInfo
{
	public MotionData anim;
	public MotionData mixamo_anim;

	public override void Awake()
	{
		base.Awake();
	}

	public void Init(MotionButtonInfo animation_)
	{
		text.text = animation_.motion.name.Replace(".motion","");
		anim = animation_.motion;
		mixamo_anim = animation_.mixamo;
		gameObject.name = animation_.motion.name;
		titleImage.sprite = animation_.image;
	}

	public override void ClickButton()
	{
		string model_name = SelectionController.currentModel.name;

		if(model_name == "smpl" || model_name == "smpl_clone")
			SelectionController.currentMotion = anim;
		else
			SelectionController.currentMotion = mixamo_anim;
		controller.frame = 0;
	}
}
}