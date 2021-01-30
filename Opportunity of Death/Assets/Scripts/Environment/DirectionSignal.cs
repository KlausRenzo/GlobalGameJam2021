using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionSignal : MonoBehaviour
{
	[SerializeField] Transform directionToPoint;

	[SerializeField] private Material[] materials;



	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}


	private void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<Player>())
			return;
	}
}