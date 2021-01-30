using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] private List<GameObject> leds;


	[SerializeField] private Material off;

	[SerializeField] private Material green;

	[SerializeField] private Material orange;

	[SerializeField] private Material red;

	[SerializeField] private int orangeHealth;
	[SerializeField] private int redHealth;


	private int health;

	public int Health
	{
		get => health;
		set
		{
			health = value;
			UpdateLeds(value);
		}
	}


	// Start is called before the first frame update
	void Start()
	{
		health = leds.Count;
	}

	// Update is called once per frame
	void Update()
	{
	}

	[Button]
	public void UpdateLeds(int value)
	{
		Material newMaterial = off;
		if (value > orangeHealth)
		{
			// Full Health
			newMaterial = green;
		}
		else if (value > redHealth)
		{
			// Medium Health
			newMaterial = orange;
		}
		else
		{
			// Low Health
			newMaterial = red;
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
}