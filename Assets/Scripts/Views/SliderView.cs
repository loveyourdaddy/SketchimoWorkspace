using Sketchimo.Controllers;
using Sketchimo.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sketchimo.Views
{
public class SliderView : MonoBehaviour
{
    public MotionController motionController;
    public MotionInfo motionModel;
    public Slider slider;

    public Text totalFrameText;
    public Text currentFrameText;
    public Text playSpeedText;

    public GridLayoutGroup lines;
    public GameObject line_obj;

    private float playSpeed = 1.0f;
    private bool pointerdown = false;
    private float timerValue = 0;

    private RectTransform rect;

	private void Start()
	{
        StartCoroutine(Start_Coroutine());
        rect = GetComponent<RectTransform>();
    }

    public IEnumerator Start_Coroutine()
	{
        yield return new WaitUntil(() => motionModel.GetTotalFrame() != 0);

        SliderLine(motionController.TotalFrame);
        totalFrameText.text = motionController.TotalFrame.ToString();
        slider.maxValue = motionController.TotalFrame;
    }

    public void Update()
    {
        ApplyAimation();
            
        currentFrameText.text = motionController.CurrentFrame.ToString();
     }

    public void SliderLine(int index)
	{
            Transform[] childList = lines.transform.GetComponentsInChildren<Transform>();

            if(childList != null)
                for(int i = 1; i < childList.Length; i++)
                    if (childList[i] != lines.transform)
                        Destroy(childList[i].gameObject);

            for(int i = 0; i < index; i++)
                Instantiate(line_obj).transform.SetParent(lines.transform);

            lines.spacing = new Vector2(rect.rect.width / index - lines.cellSize.x, 0);
	}

    public void ApplyAimation()
	{
        if (!pointerdown && Timer())
        {
            switch (motionController.CurrentPlay)
            {
                case Sketchimo.Controllers.PlayState.Play:
                    motionController.SetFrameByValue(1);
                    break;

                case Sketchimo.Controllers.PlayState.Backward:
                    motionController.SetFrameByValue(-1);
                    break;

                default:
                    break;
            }

            float tempTimeRate = (float)(motionController.CurrentFrame);
            slider.SetValueWithoutNotify(tempTimeRate);
        }
    }

    public void OnValueChanged(float value)
    {
        motionController.SetFrameFromSlider(value);
    }

    public void SetPlaySpeed(float value)
	{
        playSpeed = value;
        playSpeedText.text = value.ToString("0.0");
	}

    public bool Timer()
    {
        timerValue += Time.deltaTime;

        if (timerValue >= 1.0f / motionModel.motion.fps / playSpeed)
        {
            timerValue = 0f;
            return true;
        }
        return false;
    }

    public void PointerDown()
    {
        pointerdown = true;
    }

    public void PointerUp()
    {
        pointerdown = false;
    }
}
}