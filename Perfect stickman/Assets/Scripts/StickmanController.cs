using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;

public class StickmanController : MonoBehaviour
{

    [SerializeField] private GameObject IndicatorPrefab;

	[SerializeField] private float indicatorRadius = .5f;

	public bool isUiElement;

	#region Input
	[SerializeField] private GameInputs inputs;
	private Vector2 mousePos;
	private Vector2 mouseScreenPos;
	#endregion


	[SerializeField] private Camera uiCamera;
	[SerializeField] private RawImage rawImage;

	public List<HingeJoint2D> _joints;
	public List<JointBehaviour> _jointScripts;

	public List<JointIndicatorScript> _jointIndicatorsScripts;

	private VariableManager varM;

	void Start()
	{
		if (!isUiElement) InitiateJoints();
		inputs = new();
		inputs.Enable();
		//InstantiateJointIndicators();
		_jointIndicatorsScripts = GetComponentsInChildren<JointIndicatorScript>().ToList();
		varM = FindAnyObjectByType<VariableManager>();
	}
	void Update()
	{
		MousePos();
		ManageInputs();
		SetAction();
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
	private void InitiateJoints()
	{
		_joints = transform.GetComponentsInChildren<HingeJoint2D>().ToList();
		_jointScripts = transform.GetComponentsInChildren<JointBehaviour>().ToList();
	}

	private void SetAction()
	{
		foreach (var currIndicator in _jointIndicatorsScripts)
		{
			JointBehaviour currJointScript = currIndicator.myJointScript;

			if (!CheckMouseRange(currIndicator.transform))
			{
				currIndicator.Hide();

				continue;
			}
			currIndicator.Show();

			if (inputs.Player.Hold.IsPressed())				currJointScript.ChangeTo_Hold();
			else if (inputs.Player.Relax.IsPressed())		currJointScript.ChangeTo_Relax();
			else if (inputs.Player.RotateLeft.IsPressed())	currJointScript.ChangeTo_Rotate(1);
			else if (inputs.Player.RotateRight.IsPressed()) currJointScript.ChangeTo_Rotate(-1);
		}

		bool CheckMouseRange(Transform pos)
		{
			if ((mousePos - (Vector2)pos.position).magnitude <= indicatorRadius) return true;
			else return false;
		}

		/*void Tap()
		{
			if (jointScript.CurrState != 0) jointScript.CurrState = 0;
			else jointScript.CurrState = 1;
		}
		void Drag()
		{
			jointScript.CurrState = 2;
			int offset = (int)Mathf.Sign(mouseScreenPos.x - mouseDownPos.x);

			jointScript.rotationSpeed = offset * 1000;
		}*/
	}
	//Inputs
	private void MousePos()
	{
		if (!isUiElement)
		{
			mouseScreenPos = Mouse.current.position.ReadValue();
			mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
		}
		else
		{
			GetMouseImagePos();
		}

		void GetMouseImagePos()
		{
			Vector2 imageScreenPos = rawImage.transform.position;
			Vector2 imageWorldPos = Camera.main.ScreenToWorldPoint(imageScreenPos);
			float uiCamScale = uiCamera.orthographicSize;
			Vector2 uiCamPos = uiCamera.transform.position;

			Vector2 imageScale = (Vector2)Camera.main.ScreenToWorldPoint(imageScreenPos + rawImage.rectTransform.sizeDelta / 2) - imageWorldPos;

			float sizeRatio = uiCamScale / imageScale.x;

			Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			//print("asef: " + mousePos);
			Vector2 output = (mousePos - imageWorldPos) * sizeRatio + uiCamPos;
			Vector2 outputScreen = Camera.main.WorldToScreenPoint(output);

			Debug.DrawLine(mousePos, output, Color.blue);

			this.mousePos = output;
			this.mouseScreenPos = outputScreen;
		}
	}
	private void ManageInputs()
	{
	}
}
