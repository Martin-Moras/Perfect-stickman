using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointIndicatorScript : MonoBehaviour
{
    public JointBehaviour myJointScript;
    public HingeJoint2D myJoint;
    public SpriteRenderer renderer;

	public bool isUiElement;

	private VariableManager varM;

	

	void Start()
    {
		if (!isUiElement)
		{
			myJointScript = transform.parent.GetComponent<JointBehaviour>();
			myJoint = transform.parent.GetComponent<HingeJoint2D>();
		}
		varM = FindFirstObjectByType<VariableManager>();
		renderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
		Indicator();

	}

	private void Indicator()
	{
		UpdatePos();
		UpdateSprite();

		
		void UpdateSprite()
		{
			if (myJointScript.CurrState == 0) renderer.sprite = varM.holdSprite;
			else if (myJointScript.CurrState == 1) renderer.sprite = varM.relaxSprite;
			else if (myJointScript.CurrState == 2) renderer.sprite = varM.rotateSprite;
			else if (myJointScript.CurrState == 3) renderer.sprite = varM.rotateTowartsSprite;
		}
		void UpdatePos()
		{
			if (isUiElement) return;

			Vector2 jointPos = (Vector2)(Quaternion.Euler(0, 0, myJoint.transform.eulerAngles.z) * (myJoint.connectedAnchor * myJoint.connectedBody.transform.lossyScale)) + (Vector2)myJoint.transform.position;
			transform.position = jointPos;
		}
	}
	public void Show()
	{
		if (isUiElement)
		{
			transform.localScale = Vector2.one * varM.selectedScale;
			renderer.enabled = true;
		}

		else renderer.enabled = true;
	}
	public void Hide()
	{
		if (isUiElement)
		{
			transform.localScale = Vector2.one * varM.unSelectedScale;
			renderer.enabled = true;
		}
		else renderer.enabled = false;
	}
}
