using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JointBehaviour : MonoBehaviour
{
	//public float targetAngle;
	public float rotationSpeed;
	public float motorTorque;

	public float currState = 3;
	public enum State
	{
		Hold = 0,
		Relax = 1,
		Rotate = 2,
		RotateTowards = 3
	}
	[HideInInspector] public GameObject indicator;
	

	private Rigidbody2D rb;
	private HingeJoint2D joint;
	private JointMotor2D motor;
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		joint = GetComponent<HingeJoint2D>();
		motor = joint.motor;
	}
	void FixedUpdate()
	{
		JointAction();
	}
	void Update()
	{
	}
	public void JointAction()
	{
		if (currState == (int)State.Hold) Hold();
		else if (currState == (int)State.Relax) Relax();
		else if (currState == (int)State.Rotate) Rotate(rotationSpeed);
		//else if (currState == (int)State.RotateTowards) RotateTowards(targetAngle);
	}
	void Hold()
	{
		SetMotorSpeed(0);
		SetMotorTorque(motorTorque);
	}
	void Relax()
	{
		SetMotorTorque(0);
	}
	void Rotate(float speed)
	{
		SetMotorTorque(motorTorque);
		SetMotorSpeed(speed);
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