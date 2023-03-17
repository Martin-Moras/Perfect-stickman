using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDTest : MonoBehaviour
{
	[SerializeField] private Transform targetT;
	[SerializeField] private float maxForce;
	[SerializeField] private bool manualControll;
	[SerializeField] private float testValue;


	private float timeStep;
	private float target;

	private Rigidbody2D rb;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		timeStep = Time.fixedDeltaTime;
	}
	private void Update()
	{
		Resett();
	}
	private void FixedUpdate()
	{
		target = targetT.eulerAngles.z;
		MovePointer();
		ManualControll();
	}
	private void LateUpdate()
	{
	}
	private void MovePointer()
	{
		if (manualControll) return;

		float currPos = transform.position.x;

		float distanceUntil0 = DistanceUntil_0(rb.velocity.x);

		//Get dir
		int dir = (int)Mathf.Sign(target - currPos);
		bool isOvershooting = IsInbetween(currPos, currPos + distanceUntil0, target);
		if (isOvershooting) dir *= -1;

		float futureVelocity = AddForce_Simulated(rb.velocity.x, maxForce * dir, rb.mass);
		float futurePosition = currPos + rb.velocity.x * timeStep;
		float futureTargetDistance = target - futurePosition;

		float targetDistance = target - currPos;

		/*print(
			//$"Target dist: {targetDistance}\n" +
			$"distance {distanceUntil0}\n" +
			$"Vel + a: {DistanceUntil_0(futureVelocity)}\n" +
			$"\n" +
			$"\n"
			);*/

		//float force = GetSpeed();
		float force = GetSpeed();


		Mover(force * dir);
		Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * distanceUntil0, Color.red);





		float DistanceUntil_0(float v0)
		{
			float a = maxForce * -Mathf.Sign(v0) / rb.mass;
			return -v0*v0 / (2*a);
		}
		float GetSpeed()
		{
			if (isOvershooting) return maxForce;
			if (!IsInbetween(distanceUntil0, DistanceUntil_0(futureVelocity), futureTargetDistance)) return maxForce;

			float v0 = rb.velocity.x;
			float d = targetDistance;
			float a = -v0 * v0 / (2 * d);
			print(a);

			return a;

			/*print("overshoot");
			float a = maxForce * -Mathf.Sign(v0);
			//t = how many steps it takes untill velocity = 0
			float t = Mathf.Abs(v0 / a);
			float d = distanceUntil0 - targetDistance;
			return (d - v0 * t) * 2 / t*t;*/
		}
		float AddForce_Simulated(float velocity, float force, float mass)
		{
			return velocity + (force * timeStep / mass);
		}
	}
	private void ManualControll()
	{
		if (!manualControll) return;

		float dir = 0;

		if (Input.GetKey(KeyCode.D)) dir = 1;
		if (Input.GetKey(KeyCode.A)) dir = -1;
		if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) dir = 0;
		
		Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * DistanceUntil_0(rb.velocity.x), Color.red);
		/*print(
			$"t: {t}\n" +
			$"d: {d}\n" +
			$"a: {a}\n" +
			$"v0: {v0}\n" +
			$"\n" +
			$"\n"
			);*/
		Mover(dir * maxForce);

		float DistanceUntil_0(float v0)
		{
			v0 *= rb.mass;
			float a = maxForce * -Mathf.Sign(v0);
			return -v0 * v0 / (2 * a);
		}
	}
	private void Resett()
	{
		if (!Input.GetKeyDown(KeyCode.R)) return;

		rb.velocity = Vector2.zero;
		transform.position = Vector2.right * 10;
	}
	private void Mover(float force)
	{
		rb.AddForce(Vector2.right * force);

		Draw();
		void Draw()
		{	
			if (force < 0) Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * force, Color.green);
			if (force > 0) Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * force, Color.green);
		}  
	}
	private bool IsInbetween(float a, float b, float point)
	{
		return (a < point && b > point) || (a > point && b < point);
	}
}
