using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float torque;
	[SerializeField] private new Rigidbody rigidbody;
	[SerializeField] private Transform body;
	[SerializeField] private LayerMask terrainLayerMask;

	// Start is called before the first frame update
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
		Controls();
		AdaptToTerrain();
		body.transform.position = rigidbody.position;
	}

	private void AdaptToTerrain()
	{
		Ray ray = new Ray(body.position, -body.up * 3);
		Debug.DrawRay(ray.origin, Vector3.down);

		Physics.Raycast(ray, out RaycastHit hit, 500f, terrainLayerMask);

		body.up = Vector3.Lerp(body.up, hit.normal, 0.01f);
	}

	private void Controls()
	{
		float horizontalAxis = Input.GetAxis("Horizontal");
		float verticalAxis = Input.GetAxis("Vertical");

		rigidbody.AddForce(body.transform.forward * torque * verticalAxis);
		rigidbody.AddForce(body.transform.right * torque * horizontalAxis);
	}
}