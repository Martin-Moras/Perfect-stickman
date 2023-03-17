using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
	[SerializeField] private float timeScale;
	[SerializeField] private float stepSpeed;
	[SerializeField] private int stepAmount;

	private int stepedFrames;
	private bool wasPressedLastFrame;

	[SerializeField] private GameInputs inputs;

	void Start()
	{
		inputs = new();
		inputs.Enable();
	}
	void Update()
	{
		Inputs();
		SetTimeScale();
	}
	private void FixedUpdate()
	{
		StepFrame();
	}
	private void OnEnable()
	{
		inputs = new();
		inputs.Enable();
	}
	private void OnDisable()
	{
		inputs.Disable();
	}

	private void StepFrame()
	{
		if (stepedFrames == 0) return;

		Time.timeScale = stepSpeed;
		timeScale = stepSpeed;

		stepedFrames -= 1;
		if (stepedFrames == 0)
		{
			Time.timeScale = 0;
			timeScale = 0;
		}
	}
	private void SetTimeScale()
	{
		Time.timeScale = timeScale;
	}
	private void Inputs()
	{
		timeScale += inputs.Frames.timeScale.ReadValue<float>() * .1f;
		if (timeScale < 0) timeScale = 0;
		
		if (inputs.Frames.NextFrame.IsPressed() && !wasPressedLastFrame)
		{
			wasPressedLastFrame = true;
			stepedFrames = stepAmount + 1;
			StepFrame();
		}
		else if (!inputs.Frames.NextFrame.IsPressed()) wasPressedLastFrame = false;
	}
}
