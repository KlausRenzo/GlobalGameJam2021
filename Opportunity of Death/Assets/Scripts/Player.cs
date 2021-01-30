using System.Collections;
using System.Collections.Generic;
using Aura2API;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float maxSpeed = 1f;
	public float acceleration = 1f;

	public float steeringSpeed = 1f;


	[SerializeField] private float torque;
	[SerializeField] private new Rigidbody rigidbody;
	[SerializeField] private LayerMask terrainLayerMask;


	public float speed;
	public float rotation;

	// Start is called before the first frame update
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
		Controls();
		AdaptToTerrain();
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



		transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

		//transform.up = hit.normal;
		//transform.Rotate(transform.up, rotation);

		transform.up = hit.normal;
		Vector3 euler = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(euler.x, rotation, euler.z);
	}

	private void Controls()
	{
		float horizontalAxis = Input.GetAxis("Horizontal");
		float verticalAxis = Input.GetAxis("Vertical");

		speed = Mathf.Lerp(speed, verticalAxis * maxSpeed, acceleration * Time.deltaTime);
		transform.position += transform.forward * speed;
		rotation += horizontalAxis * steeringSpeed * Time.deltaTime;
	}
}