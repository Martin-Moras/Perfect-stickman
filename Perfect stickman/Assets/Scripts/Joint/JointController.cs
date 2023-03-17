using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;
using UnityEngine.UI;

public class JointController : MonoBehaviour
{
	float showScale = 1.5f;
	float hideScale = 1f;
	#region Input
	private Vector2 mousePos;
	private Vector2 mouseScreenPos;
	//private Vector2 mouseImagePos;
	private Vector2 mouseDownPos;
	private bool jointInputIsPressed;
	private bool isIndicatorShown;
	private bool isMouseClicked;
	private bool mouseInRange = false;
	#endregion

	public bool isUiElement = false;

	[HideInInspector] public RawImage rawImage;
	[HideInInspector] public Camera uiCamera;
	public HingeJoint2D joint;
	private JointBehaviour jointScript;
	private SpriteRenderer spriteRenderer;
	private JointController jointIndicatorScript;

	[SerializeField] private float indicatorRadius = .5f;
	[SerializeField] private GameInputs inputs;

	#region Sprite
	[SerializeField] private Sprite holdSprite;
	[SerializeField] private Sprite relaxSprite;
	[SerializeField] private Sprite rotateSprite;
	[SerializeField] private Sprite rotateTowartsSprite;
	#endregion

	void Start()
	{
		if (joint == null) joint = transform.parent.GetComponent<HingeJoint2D>();
		jointScript = joint.GetComponent<JointBehaviour>();
		jointIndicatorScript = transform.GetComponentInChildren<JointController>();
		spriteRenderer = jointIndicatorScript.GetComponent<SpriteRenderer>();
		inputs = new();
		inputs.Enable();
		if (isUiElement) spriteRenderer.enabled = true;
		else spriteRenderer.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		ManageInputs();
		MousePos();
		SetAction();
		Indicator();
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
	private void SetAction()
	{
		/*print($"isMouseClicked: {isMouseClicked}\n" +
			$"hold?: {hold}\n" +
			$"mouseInRange: {mouseInRange}");*/

		//Debug.DrawLine(mousePos, Camera.main.ScreenToWorldPoint(mouseDownPos));

		if(!isMouseClicked && jointInputIsPressed && mouseInRange)
		{
			mouseDownPos = mouseScreenPos;
			isMouseClicked = true;
		}
		else if(isMouseClicked && !jointInputIsPressed)
		{
			float tolerance = 3f;

			isMouseClicked = false;
			if ((mouseScreenPos - mouseDownPos).magnitude <= tolerance) Tap();
			else Drag();
		}
		void Tap()
		{
			if (jointScript.currState != 0) jointScript.currState = 0;
			else jointScript.currState = 1;
		}
		void Drag()
		{
			jointScript.currState = 2;
			int offset = (int)Mathf.Sign(mouseScreenPos.x - mouseDownPos.x);

			jointScript.rotationSpeed = offset * 1000;

			//jointScript.SetTargetAngle(Mathf.Atan2(offset.y, -offset.x) * Mathf.Rad2Deg);
		}
	}
	
	private void Indicator()
	{
		ShowIndicator();

		void ShowIndicator()
		{
			//Vector2 jointPos = (Vector2)(Quaternion.Euler(0, 0, joint.connectedBody.rotation) * (joint.connectedAnchor * joint.connectedBody.transform.lossyScale)) + joint.connectedBody.position;
			Vector2 jointPos = transform.position;
			if (
				(jointPos - mousePos).magnitude < indicatorRadius ||
				(isIndicatorShown && jointInputIsPressed)
				)
				Show();
			else Hide();

			if(transform.parent.name == "Head" && isUiElement) print($"joint pos: {jointPos}\n" +
				$"mousepos: {mousePos}\n" +
				$"magnitute: {(jointPos - mousePos).magnitude}\n" +
				$"\n"
				);

			UpdatePos();
			UpdateSprite();

			void Show()
			{

				isIndicatorShown = true;
				mouseInRange = true;

				if (isUiElement)
				{
					transform.localScale = Vector2.one * showScale;
				}

				else spriteRenderer.enabled = true;
			}
			void Hide()
			{

				mouseInRange = false;
				isIndicatorShown = false;

				if (isUiElement)
				{
					transform.localScale = Vector2.one * hideScale;
				}
				else	spriteRenderer.enabled = false;
			}
			void UpdateSprite()
			{
				if (jointScript.currState == 0) spriteRenderer.sprite = holdSprite;
				else if (jointScript.currState == 1) spriteRenderer.sprite = relaxSprite;
				else if (jointScript.currState == 2) spriteRenderer.sprite = rotateSprite;
				else if (jointScript.currState == 3) spriteRenderer.sprite = rotateTowartsSprite;
			}
			void UpdatePos()
			{
				if (isUiElement) return;

				Vector2 jointPos = (Vector2)(Quaternion.Euler(0, 0, joint.transform.eulerAngles.z) * (joint.connectedAnchor * joint.connectedBody.transform.lossyScale)) + (Vector2)joint.transform.position;
				transform.position = jointPos;
			}
		}
	}
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
			Vector2 uiCam_Image_Distance = uiCamera.transform.position - Camera.main.ScreenToWorldPoint(imageScreenPos);

			Vector2 imageScale = (Vector2)Camera.main.ScreenToWorldPoint(imageScreenPos + rawImage.rectTransform.sizeDelta / 2) - imageWorldPos;

			float sizeRatio = uiCamScale / imageScale.x;

			Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			//print("asef: " + mousePos);
			Vector2 output = (mousePos - imageWorldPos) * sizeRatio + uiCam_Image_Distance + imageWorldPos;
			Vector2 outputScreen = Camera.main.WorldToScreenPoint(output);

			//Debug.DrawLine(mousePos, outputScreen, Color.red);

			this.mousePos = output;
			this.mouseScreenPos = outputScreen;
		}
	}
	private void ManageInputs()
	{
		jointInputIsPressed = inputs.Player.ControllJoint.IsPressed();
	}
}