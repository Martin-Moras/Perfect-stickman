using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JointBehaviour : MonoBehaviour
{
	//public float targetAngle;
	public float maxRotationSpeed;
	[SerializeField] private float rotateDirection;
	public float motorTorque;

	public bool isPrediction = false;

	[SerializeField] private float propCurrState;

	public float CurrState
	{
		get { return propCurrState; }
		private set 
		{
			propCurrState = value;
			if (isPrediction) return;
			FindFirstObjectByType<PredictFuture>().ResetScene();
		}
	}

	private enum State
	{
		Hold = 0,
		Relax = 1,
		Rotate = 2,
		RotateTowards = 3
	}
	private HingeJoint2D joint;
	private JointMotor2D motor;


	void Start()
	{
		joint = GetComponent<HingeJoint2D>();
		motor = joint.motor;

		if (gameObject.scene.name == "Simulation") isPrediction = true;

		if (isPrediction) FindFirstObjectByType<FrameManager>().PredictionFrame += OnPredictionFrame;
		else FindFirstObjectByType<FrameManager>().PhysicFrame += OnPhysicFrame;
	}
	private void OnDestroy()
	{
		var frameManager = FindFirstObjectByType<FrameManager>();

		if (frameManager == null)
		{
			Console.WriteLine("frameManager == null");
			return;
		}

		frameManager.PhysicFrame -= OnPhysicFrame;
		frameManager.PredictionFrame -= OnPredictionFrame;
	}
	public void OnPhysicFrame(object sorce, EventArgs e)
	{
		JointAction();
	}
	public void OnPredictionFrame(object sorce, EventArgs e)
	{
		JointAction();
	}

	public void ChangeTo_Hold()
	{
		CurrState = 0;
	}
	public void ChangeTo_Relax()
	{
		CurrState = 1;
	}
	public void ChangeTo_Rotate(int direction)
	{
		CurrState = 2;
		rotateDirection = Mathf.Sign(direction);
	}
	public void JointAction()
	{
		if (CurrState == (int)State.Hold) Hold();
		else if (CurrState == (int)State.Relax) Relax();
		else if (CurrState == (int)State.Rotate) Rotate();
		//else if (currState == (int)State.RotateTowards) RotateTowards(targetAngle);
	}
	private void Hold()
	{
		SetMotorSpeed(0);
		SetMotorTorque(motorTorque);
	}
	private void Relax()
	{
		SetMotorTorque(0);
	}
	private void Rotate()
	{
		SetMotorSpeed(maxRotationSpeed * rotateDirection);
		SetMotorTorque(motorTorque);
	}

	private float ToAngle(float angle)
	{
		angle = angle % 360;
		if (angle < 0)
		{
			angle *= -1;
			angle = angle % 360;
			angle += 360 - 2 * angle;
		}
		return angle;
	}
	private void SetMotorSpeed(float speed)
	{
		motor.motorSpeed = speed;
		joint.motor = motor;
	}
	private void SetMotorTorque(float torque)
	{
		motor.maxMotorTorque = torque;
		joint.motor = motor;
		
	}
	/*void RotateTowards(float targetAngle)
	{
		SetMotorTorque(motorTorque);

		targetAngle = ToAngle(targetAngle);
		float currentAngle = ToAngle(joint.jointAngle);
		float error = -((currentAngle - targetAngle + 540) % 360 - 180);

		float desiredVelocity = error * rotationSpeed;
		float currentVelocity = rb.angularVelocity;
		float torque = motor.maxMotorTorque;


		if (Mathf.Abs(error) > 1) 
		{
			float dampingForce = -currentVelocity * damping;
			SetMotorSpeed(desiredVelocity + dampingForce);
		}
		else
		{
			SetMotorSpeed(0);
		}
	}*/
	/*public enum ReferenceSpace
	{
		Local = 0,
		World = 1
	}
	public void SetTargetAngle(float alngle, ReferenceSpace space = ReferenceSpace.World)
	{
		if (space == ReferenceSpace.Local) targetAngle = alngle;
		else
		{
			targetAngle = alngle - (joint.connectedBody.rotation % 360) * 2;
			print($"alngle: {alngle}\n" +
			$"rotatoin?: {joint.connectedBody.rotation}");
		}
	}*/
}