using System;
using System.Collections;
using System.Collections.Generic;
using Aura2API;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float maxSpeed = 1f;
	[SerializeField] private float acceleration = 1f;
	[SerializeField] private float steeringSpeed = 1f;

	[SerializeField] private LayerMask terrainLayerMask;
	
	[SerializeField] private float speed;
	[SerializeField] private float rotation;

	private Animator animator;
	private RoverEnergy roverEnergy;
	private RoverInteraction roverInteraction;

	public bool controlEnabled = true;
	public event Action InteractButtonPressed;

	private void Start()
	{
		roverEnergy = GetComponent<RoverEnergy>();
		roverInteraction = GetComponent<RoverInteraction>();
		animator = GetComponent<Animator>();

		roverEnergy.EnergyLost += RoverEnergyOnEnergyLost;
	}

	private void RoverEnergyOnEnergyLost()
	{
		if (roverEnergy.Energy == 0)
		{
			controlEnabled = false;
		}

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
		transform.position = new Vector3(transform.position.x, hit.point.y + 0.3f, transform.position.z);
	}

	private void Controls()
	{
		float horizontalAxis = Input.GetAxis("Horizontal");
		float verticalAxis = Input.GetAxis("Vertical");
		if(Input.GetButtonDown("Interaction"))
			InteractButtonPressed?.Invoke();
		

		speed = Mathf.Lerp(speed, verticalAxis * maxSpeed, acceleration * Time.deltaTime);

		Vector3 deltaPostion = transform.forward * speed * maxSpeed;
		transform.position += deltaPostion;
		roverEnergy.LoseRange(deltaPostion.magnitude);
		rotation += horizontalAxis * steeringSpeed * Time.deltaTime;
	}
}