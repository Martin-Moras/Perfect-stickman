using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PredictFuture : MonoBehaviour
{
	private Scene _simulationScene;
	private Scene _mainScene;
	private PhysicsScene2D _physicsScene;

	private float _simulationTime;

	private VariableManager varM;

	private void Start()
	{
		varM = FindAnyObjectByType<VariableManager>();
		_mainScene = SceneManager.GetActiveScene();
		CreatePhysicsScene();
		GameObject.FindFirstObjectByType<FrameManager>().PredictionFrame += OnPredictionFrame;
	}
	public void OnPredictionFrame(object sorce, EventArgs e)
	{
		SimulateScene();
	}

	private void CreatePhysicsScene()
	{
		if (!_simulationScene.isLoaded) _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
		_physicsScene = _simulationScene.GetPhysicsScene2D();

		foreach (var obj in GetAllPhisicObjects())
		{
			var ghostObj = Instantiate(obj.gameObject, obj.transform.position, obj.transform.rotation);
			SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
			
			ghostObj.layer = 8;

			List<Transform> ghostChildren = ghostObj.GetComponentsInChildren<Transform>().ToList();
			ghostChildren.Add(ghostObj.transform);
			
			List<Transform> objChildren = obj.GetComponentsInChildren<Transform>().ToList();
			objChildren.Add(obj.transform);

			for(int i = 0; i < ghostChildren.Count; i++)
			{
				ChangeColor(ghostChildren[i]);
				SetupRigidbody(i);

			}
			
			
			
			void ChangeColor(Transform obj)
			{
				SpriteRenderer renderer;
				obj.TryGetComponent(out renderer);

				if (renderer == null) return;
				
				renderer.color = varM._simulationColor;
				renderer.sortingLayerName = "Prediction";
			}
			void SetupRigidbody(int index)
			{
				Rigidbody2D ghostRb;
				ghostChildren[index].TryGetComponent(out ghostRb);
				if (ghostRb == null) return;

				Rigidbody2D objRb = objChildren[index].GetComponent<Rigidbody2D>();

				ghostRb.velocity = objRb.velocity;
				ghostRb.angularVelocity = objRb.angularVelocity;
			}
			void SetupHingeJoints(int index)
			{
				HingeJoint2D ghostJoint;
				ghostChildren[index].TryGetComponent(out ghostJoint);
				if (ghostJoint == null) return;

				HingeJoint2D objJoint = objChildren[index].GetComponent<HingeJoint2D>();

				ghostJoint.motor = objJoint.motor;
			}
		}
		List<Transform> GetAllPhisicObjects()
		{
			List<Transform> output = new();

			foreach (var obj in _mainScene.GetRootGameObjects())
			{
				if (obj.name == "Stickman" || obj.GetComponent<BoxCollider2D>() != null)
				{
					output.Add(obj.transform);
					continue;
				}
				/*if (!obj.GetComponent<Rigidbody2D>() && !obj.GetComponent<Collider2D>()) continue;
				if (output.Contains(obj.parent)) continue;
				

				output.Add(obj);*/
			}

			return output;
		}
		
	}
	public void ResetScene()
	{
		
		_simulationTime = 0;

		foreach (GameObject gameObject in _simulationScene.GetRootGameObjects()) Destroy(gameObject);
		CreatePhysicsScene();
	}
	private void SimulateScene()
	{
		if (_simulationTime > varM._simulationDuration) ResetScene();
		if (_physicsScene.IsValid()) _physicsScene.Simulate(Time.fixedDeltaTime);
		_simulationTime += Time.fixedDeltaTime;
	}
}