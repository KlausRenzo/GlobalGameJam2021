using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoverEnergy : MonoBehaviour
{
	private Player player;
	[SerializeField] private float rangePerEnergy = 200;

	[SerializeField] private List<GameObject> leds;

	[SerializeField] private Material off;
	[SerializeField] private Material green;
	[SerializeField] private Material orange;
	[SerializeField] private Material red;

	[SerializeField] private int orangeHealth;
	[SerializeField] private int redHealth;

	[SerializeField] private int energy;
	[SerializeField] [Range(0f, 5f)] private float rangeDepletionFactor = 1f;


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

	// Update is called once per frame
	void Update()
	{
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

		int e = Mathf.FloorToInt(range / rangePerEnergy);
		if (energy != e)
			Energy = e;
	}

	public void LosePiece()
	{
		Energy -= 1;
		range -= rangePerEnergy;
	}
}