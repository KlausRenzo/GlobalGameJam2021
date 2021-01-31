using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private float maxSpeed = 1f;
	[SerializeField] private float acceleration = 1f;
	[SerializeField] private float steeringSpeed = 1f;

	[SerializeField] private LayerMask terrainLayerMask;

	[SerializeField] private float speed;
	[SerializeField] private float rotation;

	[SerializeField] private ParticleSystem[] sparaFumo;

	private Animator animator;
	private RoverEnergy roverEnergy;
	private RoverInteraction roverInteraction;

	public bool controlEnabled = true;
	public event Action InteractButtonPressed;

	private float steeringMalus = 1f;
	private float speedMalus = 1f;
	private float horizontalAxis;
	private float verticalAxis;

	private void Start()
	{
		roverEnergy = GetComponent<RoverEnergy>();
		roverInteraction = GetComponent<RoverInteraction>();
		animator = GetComponent<Animator>();

		roverEnergy.EnergyLost += RoverEnergyOnEnergyLost;
	}

	private void RoverEnergyOnEnergyLost()
	{
		if (roverEnergy.Energy <= 0)
		{
			GameManager.Instance.ActivateSleepMode(this);
		}
	}

	private void Update()
	{
		if (controlEnabled)
			Controls();

		Move();
		AdaptToTerrain();
		SetAnimatorParameters();

		DebugControls();
		SmokeMachine();
	}

	private void Move()
	{
		rotation += horizontalAxis * steeringSpeed * Time.deltaTime * steeringMalus * (debugMovement ? 4 : 1);

		speed = Mathf.Lerp(speed, verticalAxis * maxSpeed * speedMalus * (debugMovement ? 4 : 1), acceleration * Time.deltaTime);
		Vector3 deltaPostion = transform.forward * speed * maxSpeed;
		transform.position += deltaPostion;

		roverEnergy.LoseRange(deltaPostion.magnitude * (debugMovement ? 1/4 : 1));
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
		horizontalAxis = Input.GetAxis("Horizontal");
		verticalAxis = Input.GetAxis("Vertical");

		if (Input.GetButtonDown("Interaction"))
			InteractButtonPressed?.Invoke();

		if (Input.GetButtonDown("Sleep"))
			GameManager.Instance.ActivateSleepMode(this);
	}

	public void AddSteeringMalus(float steeringMalus)
	{
		this.steeringMalus = steeringMalus;
	}

	public void AddSpeedMalus(float speedMalus)
	{
		this.speedMalus = speedMalus;
	}

	private void DebugControls()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			roverEnergy.LosePiece();
		}

		debugMovement = Input.GetKey(KeyCode.B);

	}

	private bool debugMovement;

	private void SmokeMachine()
	{
		if (speed > 0.1f && sparaFumo[1].isStopped)
		{
			for (int i = 0; i < sparaFumo.Length; i++)
			{
				sparaFumo[i].Play();
			}
		}

		if (speed <= 0.1f && sparaFumo[1].isPlaying)
		{
			for (int i = 0; i < sparaFumo.Length; i++)
			{
				sparaFumo[i].Stop();
			}
		}
	}

	public void Sleep()
	{
		StartCoroutine(SlowDown());
	}

	IEnumerator SlowDown()
	{
		while (speed > 0.01f)
		{
			speed = Mathf.Lerp(speed, 0, acceleration * Time.deltaTime * 10);
			yield return null;
		}

		roverEnergy.RecoverEnergy();
	}
}