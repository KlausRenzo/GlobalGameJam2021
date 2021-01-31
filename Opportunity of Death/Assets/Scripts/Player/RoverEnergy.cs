using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoverEnergy : MonoBehaviour
{
	private Player player;

	[SerializeField] private int energy;

	[Header("Values")] [SerializeField] private float rangePerEnergy = 200;
	[SerializeField] [Range(0f, 1f)] private float steeringMalus = 0.7f;
	[SerializeField] [Range(0f, 1f)] private float speedMalus = 0.7f;

	[SerializeField] [Range(0f, 5f)] private float rangeDepletionFactor = 1f;


	[Header("Led")] [SerializeField] private List<GameObject> leds;

	[SerializeField] private Material off;
	[SerializeField] private Material green;
	[SerializeField] private Material orange;
	[SerializeField] private Material red;

	[SerializeField] private int orangeHealth;
	[SerializeField] private int redHealth;

	[Header("Pieces")] [SerializeField] private List<GameObject> roverPieces;


	public int Energy
	{
		get => energy;
		set
		{
			energy = value;
			UpdateLeds(value);
		}
	}

	[SerializeField] private float range;

	// Start is called before the first frame update
	void Start()
	{
		player = this.GetComponent<Player>();
		energy = leds.Count;
		range = energy * rangePerEnergy;
		UpdateLeds(energy);
	}

	[Button]
	public void UpdateLeds(int value)
	{
		Material newMaterial;
		if (value > orangeHealth)
		{
			// Full Energy
			newMaterial = green;
		}
		else if (value > redHealth)
		{
			// Medium Energy
			newMaterial = orange;
			Debug.Log("LOW ENERGY");
		}
		else
		{
			// Low Energy
			newMaterial = red;
			Debug.Log("ENERGY CRITICAL");
		}

		int i = 0;
		foreach (GameObject led in leds)
		{
			var meshRenderer = led.GetComponent<MeshRenderer>();
			if (i < value)
			{
				meshRenderer.material = newMaterial;
			}
			else
			{
				meshRenderer.material = off;
			}

			i++;
		}
	}

	public event Action EnergyLost;

	public void LoseRange(float distance)
	{
		//todo
		range -= distance * rangeDepletionFactor;
		range = Mathf.Max(0, range);

		int e = Mathf.CeilToInt(range / rangePerEnergy);
		if (energy != e)
			Energy = e;
	}

	private int piecesLost = 0;

	public void LosePiece()
	{
		Energy -= 1;
		range -= rangePerEnergy;

		var piece = roverPieces[piecesLost];


		switch (piecesLost)
		{
			case 0:
				StartCoroutine(SpawnElement(piece, -transform.right + transform.up));
				break;
			case 1:
			case 2:
				StartCoroutine(SpawnElement(piece, transform.right + transform.up));
				break;
			case 3:
				StartCoroutine(SpawnElement(piece, -transform.right + transform.up));
				break;

			case 4:
				player.AddSteeringMalus(steeringMalus);
				StartCoroutine(SpawnElement(piece, -transform.right + transform.up));
				break;

			case 5:
				player.AddSpeedMalus(speedMalus);
				StartCoroutine(SpawnElement(piece, transform.right + transform.up));
				break;

			case 6:
				GameManager.Instance.AddGlitch();
				StartCoroutine(SpawnElement(piece, transform.up));
				break;

			case 7:
				GameManager.Instance.AddBlackFade();
				StartCoroutine(SpawnElement(piece, transform.up));
				break;
		}

		piece.SetActive(false);

		piecesLost++;
	}


	private IEnumerator SpawnElement(GameObject piece, Vector3 direction)
	{
		GameObject ragdol = Instantiate(piece, piece.transform.position, piece.transform.rotation);
		var rb = ragdol.AddComponent<Rigidbody>();
		ragdol.layer = LayerMask.NameToLayer("onlyTerrain");
		rb.AddForce(direction * 300);
		rb.AddForceAtPosition(-transform.forward * 250, transform.position + transform.up);
		ragdol.AddComponent<CapsuleCollider>();

		yield return new WaitForSeconds(0.5f);
		ragdol.layer = LayerMask.NameToLayer("Default");
	}

	public void RecoverEnergy()
	{
		int remainingPieces = leds.Count - piecesLost;

		Energy = remainingPieces;
		range = Energy * rangePerEnergy;
	}

	public float GetMaxRangeInPercent()
	{
		float result = 0;

		float max = 0;
		for (int i = 0; i <= leds.Count; i++)
		{
			max += i * rangePerEnergy;
		}

		for (int i = 0; i <= leds.Count - piecesLost; i++)
		{
			result += i * rangePerEnergy;
		}

		return (result / max) * 0.8f;
	}
}