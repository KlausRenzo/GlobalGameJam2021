using System.Collections;
using UnityEngine;

public class Beacon : MonoBehaviour
{
	[SerializeField] Transform directionToPoint;
	[SerializeField] private Material[] materials;
	[SerializeField] private bool active;
	[SerializeField] private AnimationCurve lightAnimationCurve;
	[SerializeField] private float lightIntensity = 1;
	private new Light light;

	[SerializeField] private Transform head;
	[SerializeField] private float rotationSpeed;

	// Start is called before the first frame update
	void Start()
	{
		light = GetComponentInChildren<Light>();
		
	}

	// Update is called once per frame
	void Update()
	{
		if (!active)
			head.Rotate(head.up, rotationSpeed);
		else
		{
			Quaternion lookRotation = Quaternion.LookRotation(directionToPoint.position - head.transform.position);
			head.rotation = Quaternion.Slerp(head.rotation, lookRotation, rotationSpeed * Time.deltaTime);
		}
	}

	


	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("ENTER");
		var roverInteraction = other.GetComponent<RoverInteraction>();
		if (!roverInteraction)
			return;

		StopCoroutine(nameof(FadeLigthTo));
		StartCoroutine(FadeLigthTo(true));
		roverInteraction.EnableBeaconActivation(this);

		GameManager.Instance.ShowText("Activate Beacon");
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log("EXIT");
		var roverInteraction = other.GetComponent<RoverInteraction>();
		if (!roverInteraction)
			return;

		StopCoroutine(nameof(FadeLigthTo));
		StartCoroutine(FadeLigthTo(false));

		roverInteraction.DisableBeaconActivation(this);
		GameManager.Instance.HideText();
	}

	private IEnumerator FadeLigthTo(bool b)
	{
		float time = Time.time;

		float fadeDuration = 2f;

		while (Time.time - time < fadeDuration)
		{
			if (b)
			{
				var t = (Time.time - time) / fadeDuration;
				light.intensity = lightAnimationCurve.Evaluate(t) * lightIntensity;
			}
			else
			{
				var t = (Time.time - time) / fadeDuration;
				light.intensity = lightAnimationCurve.Evaluate(1 - t) * lightIntensity;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public bool Activate()
	{
		if (active)
			return false;

		active = true;
		return true;
	}

	private void OnDrawGizmosSelected()
	{
		
		Gizmos.DrawLine(head.transform.position, directionToPoint.transform.position);
	}
}