using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sketchimo.Models
{
	public class SelectionInfo : MonoBehaviour
{
    public Image titleImage;
	protected Text text;

	protected GameObject poolingObject;
	protected SelectionController controller;

	public virtual void Awake()
	{
		controller = FindObjectOfType<SelectionController>();
		text = transform.GetChild(0).GetComponent<Text>();
	}

	public virtual void Init(string name_)
	{
		text.text = name_;
		gameObject.name = name_;
	}

	public virtual void ClickButton()
	{
		
	}
}
}