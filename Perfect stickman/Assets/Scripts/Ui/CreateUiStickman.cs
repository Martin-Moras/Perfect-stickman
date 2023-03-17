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

		ReplaceJoints();

		foreach (var chiled in children)
		{
			SetLayer(chiled);
			RemoveComponents(chiled);
		}
		AddControllerScript();


		List<GameObject> GetChildren()
		{
			List<GameObject> output = new();
			foreach (var chiled in uiObject.GetComponentsInChildren<Transform>())
			{
				output.Add(chiled.gameObject);
			}
			return output;
		}
		void ReplaceJoints()
		{
			var joints = uiObject.GetComponentsInChildren<HingeJoint2D>().ToList();
			var oldJoints = gameObject.GetComponentsInChildren<HingeJoint2D>().ToList();
			var indicators = uiObject.GetComponentsInChildren<JointController>().ToList();

			Transform canvas = GameObject.FindAnyObjectByType<Canvas>().transform;
			Camera uiCam = canvas.GetComponentInChildren<Camera>();
			RawImage rawImage = canvas.GetComponentInChildren<RawImage>();

			foreach (var joint in joints)
			{
				Vector2 jointPos = (Vector2)(Quaternion.Euler(0, 0, joint.connectedBody.rotation) * (joint.connectedAnchor * joint.connectedBody.transform.lossyScale)) + joint.connectedBody.position;
				
				indicators[0].transform.position = jointPos;
				indicators[0].isUiElement = true;
				indicators[0].joint = oldJoints[0];
				indicators[0].uiCamera = uiCam;
				indicators[0].rawImage = rawImage;

				oldJoints.RemoveAt(0);
				indicators.RemoveAt(0);
			}
		}
		void SetLayer(GameObject chiled)
		{
			chiled.layer = 6;
		}
		void RemoveComponents(GameObject chiled)
		{
			GameObject.Destroy(chiled.GetComponent<StickmanController>());
			GameObject.Destroy(chiled.GetComponent<IgnoreCollision>());
			GameObject.Destroy(chiled.GetComponent<JointBehaviour>());
			GameObject.Destroy(chiled.GetComponent<Collider2D>());
			GameObject.Destroy(chiled.GetComponent<Joint2D>());
			GameObject.Destroy(chiled.GetComponent<Rigidbody2D>());
		}
		void AddControllerScript()
		{
			var script = uiObject.AddComponent<UiStickmanController>();
		}
	}
}