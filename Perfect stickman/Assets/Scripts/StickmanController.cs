using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEditor.U2D.Path.GUIFramework;

public class StickmanController : MonoBehaviour
{
	private List<HingeJoint2D> joints;
	private List<JointBehaviour> jointScripts;

    [SerializeField] private GameObject IndicatorPrefab;
    public Transform IndicatorParent;


	void Start()
	{
		//InitiateJoints();
		//InstantiateJointIndicators();
	}
	void Update()
	{
	}
	private void InitiateJoints()
	{
		joints = transform.GetComponentsInChildren<HingeJoint2D>().ToList();
		jointScripts = transform.GetComponentsInChildren<JointBehaviour>().ToList();
	}
	private void InstantiateJointIndicators()
	{
		foreach (var jointScript in jointScripts)
		{
			jointScript.indicator = Instantiate(IndicatorPrefab, IndicatorParent);
		}
	}
}
