using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] private List<GameObject> leds;

	[SerializeField] private Color off;
	[SerializeField] private Color green;
	[SerializeField] private Color orange;
	[SerializeField] private Color red;

	[SerializeField] private float intensity;

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

	public void UpdateLeds(int value)
	{
		Color color = off;
		if (value > orangeHealth)
		{
			// Full Health
			color = green;
		}
		else if (value > redHealth)
		{
			// Medium Health
			color = orange;
		}
		else
		{
			// Low Health
			color = red;
		}

		int i = 0;
		foreach (GameObject led in leds)
		{
			var material = led.GetComponent<MeshRenderer>().material;
			material.EnableKeyword("_EMISSION");
			if (i < value)
			{
				material.color = color;
				material.SetColor("_EmissionColor", color * intensity);
			}
			else
			{
				material.color = color;
				material.SetColor("_EmissionColor", color * 0.1f);
			}

			i++;
		}
	}
}