using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FrameManager : MonoBehaviour
{
	

	private float stopedPhysicFrames;
	private float stopedPredictionFrames;
	private float stopedStepFrames;
	private int framesToStep;

	private bool wasPressedLastFrame;

	private VariableManager varM;

	[SerializeField] private GameInputs inputs;

	#region Physic Event
	public delegate void PhysicFrameEventHandler(object sender, EventArgs args);
	public event PhysicFrameEventHandler PhysicFrame;
	protected virtual void OnPhysicFrame()
	{
		if (PhysicFrame != null) 
			PhysicFrame(this, EventArgs.Empty);
	}
	#endregion
	#region Prediction Event
	public delegate void PredictionFrameEventHandler(object sender, EventArgs args);
	public event PredictionFrameEventHandler PredictionFrame;
	protected virtual void OnPredictionFrame()
	{
		if (PredictionFrame != null) 
			PredictionFrame(this, EventArgs.Empty);
	}
	#endregion


	void Start()
	{
		inputs = new();
		inputs.Enable();
		varM = FindAnyObjectByType<VariableManager>();
	}
	void Update()
	{
		Inputs();
		
	}
	private void FixedUpdate()
	{
		ManageEvents();

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
	private void ManageEvents()
	{
		ManagePredictionFrame();
		ManagePhysicFrame();
		
		
		void ManagePhysicFrame()
		{
			if (varM.timeScale == 0) return;
			float delay = 1 / varM.timeScale;

			stopedPhysicFrames++;
			
			while (stopedPhysicFrames > delay * Time.fixedDeltaTime)
			{
				stopedPhysicFrames -= delay;

				OnPhysicFrame();
			}
		}
		void ManagePredictionFrame()
		{
			if (varM.predictionTimeScale == 0) return;
			float delay = 1 / varM.predictionTimeScale;

			stopedPredictionFrames++;
			while (stopedPredictionFrames > delay * Time.fixedDeltaTime)
			{
				stopedPredictionFrames -= delay;

				OnPredictionFrame();
			}
		}
	}

	private void StepFrame()
	{
		if (framesToStep == 0) return;
		framesToStep--;

		OnPhysicFrame();

		varM.timeScale = 0;

		float delay = 1 / varM.stepSpeed;

		stopedStepFrames++;
		while (stopedPhysicFrames > delay * Time.fixedDeltaTime)
		{
			stopedPhysicFrames -= delay;

			OnPhysicFrame();
		}
	}
	private void SpacePressed(bool isPressed)
	{
		if (isPressed && !wasPressedLastFrame)
		{
			wasPressedLastFrame = true;
			framesToStep = varM.stepAmount;
		}
		else if (!isPressed) wasPressedLastFrame = false;
	}
	private void Inputs()
	{
		varM.timeScale += inputs.Frames.timeScale.ReadValue<float>() * .01f;
		if (varM.timeScale < 0) varM.timeScale = 0;

		SpacePressed(inputs.Frames.NextFrame.IsPressed());
	}
}
