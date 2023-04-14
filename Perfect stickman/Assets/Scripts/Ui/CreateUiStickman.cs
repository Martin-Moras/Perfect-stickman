using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public static class CreateUiStickman
{
	public static void CreateStickman(GameObject gameObject, Transform parent)
	{
		var uiObject = GameObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity, parent);
		List<GameObject> children = GetChildren();
		var stickmanController = uiObject.GetComponent<StickmanController>();
		var oldJoints = gameObject.GetComponentsInChildren<HingeJoint2D>().ToList();

		SetupStickmanController();
		ReplaceJoints();

		foreach (var chiled in children)
		{
			SetLayer(chiled);
			RemoveComponents(chiled);
		}


		List<GameObject> GetChildren()
		{
			List<GameObject> output = new();
			foreach (var chiled in uiObject.GetComponentsInChildren<Transform>())
			{
				output.Add(chiled.gameObject);
			}
			return output;
		}
		void SetupStickmanController()
		{
			stickmanController.isUiElement = true;
			stickmanController.gameObject.name = "Stickman Ui";
			stickmanController._joints = gameObject.GetComponentsInChildren<HingeJoint2D>().ToList();
			stickmanController._jointScripts = gameObject.GetComponentsInChildren<JointBehaviour>().ToList();
		}
		void ReplaceJoints()
		{
			var indicators = uiObject.GetComponentsInChildren<JointIndicatorScript>().ToList();
			var oldIndicators = gameObject.GetComponentsInChildren<JointIndicatorScript>().ToList();


			for (int i = 0; i < indicators.Count; i++)
			{
				var joint = oldIndicators[i].myJoint;

				Vector2 jointPos = (Vector2)(joint.connectedBody.transform.rotation * (joint.connectedAnchor * joint.connectedBody.transform.lossyScale)) + joint.connectedBody.position;


				indicators[i].myJointScript = oldIndicators[i].myJointScript;
				indicators[i].myJoint = joint;
				indicators[i].transform.position = jointPos;
				indicators[i].isUiElement = true;
				stickmanController.isUiElement = true;
				indicators[i].GetComponent<SpriteRenderer>().enabled = true;

			}
		}
		void SetLayer(GameObject chiled)
		{
			chiled.layer = 6;
		}
		void RemoveComponents(GameObject chiled)
		{
			GameObject.Destroy(chiled.GetComponent<IgnoreCollision>());
			GameObject.Destroy(chiled.GetComponent<JointBehaviour>());
			GameObject.Destroy(chiled.GetComponent<JointController>());
			GameObject.Destroy(chiled.GetComponent<Collider2D>());
			GameObject.Destroy(chiled.GetComponent<Joint2D>());
			GameObject.Destroy(chiled.GetComponent<Rigidbody2D>());
		}
	}
}