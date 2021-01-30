using System.Collections;
using System.Collections.Generic;
using Aura2API;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float maxSpeed = 1f;
	public float acceleration = 1f;
	public float steeringSpeed = 1f;

	public AnimationCurve accelerationCurve;

	[SerializeField] private float torque;
	[SerializeField] private new Rigidbody rigidbody;
	[SerializeField] private LayerMask terrainLayerMask;
	private Animator animator;

	public float speed;
	public float rotation;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		Controls();
		AdaptToTerrain();
		SetAnimatorParameters();
	}

	private void SetAnimatorParameters()
	{
		animator.SetFloat("Speed", speed / maxSpeed);
		animator.SetFloat("Pitch", transform.rotation.eulerAngles.x);
	}

	private void AdaptToTerrain()
	{
		Ray ray = new Ray(transform.position + transform.up * 10, -transform.up);
		Debug.DrawRay(ray.origin, ray.direction * 10);

		if (!Physics.Raycast(ray, out RaycastHit hit, 100f, terrainLayerMask))
		{
			Debug.Log("NOT HIT");
			return;
		}

		transform.up = Vector3.Lerp(transform.up, hit.normal, Time.deltaTime * 10);
		transform.RotateAround(transform.position, transform.up, rotation);
		transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
	}

	private void Controls()
	{
		float horizontalAxis = Input.GetAxis("Horizontal");
		float verticalAxis = Input.GetAxis("Vertical");

		speed = Mathf.Lerp(speed, verticalAxis * maxSpeed, acceleration * Time.deltaTime);

		transform.position += transform.forward * speed * maxSpeed;
		rotation += horizontalAxis * steeringSpeed * Time.deltaTime;
	}
}