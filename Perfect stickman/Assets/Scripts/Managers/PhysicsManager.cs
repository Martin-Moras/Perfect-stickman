using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
	void Start()
	{
		((FrameManager)GameObject.FindFirstObjectByType(typeof(FrameManager))).PhysicFrame += OnPhysicFrame;
	}
	public void OnPhysicFrame(object sorce, EventArgs e)
	{
		Physics2D.Simulate(Time.fixedDeltaTime);
	}
}