using System.Collections;
using System.Collections.Generic;
using Aura2API;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	public static GameManager Instance => instance;

	private Camera camera;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		camera = FindObjectOfType<Camera>();
	}


	public void ActivateSleepMode(Player player)
	{
		StartCoroutine(SleepModeCoroutine(player));
	}

	private IEnumerator SleepModeCoroutine(Player player)
	{
		player.controlEnabled = false;
		player.Sleep();
		
		yield return new WaitForSeconds(2f);
		
		player.controlEnabled = true;
	}

	public void AddGlitch()
	{
		//TODO
	}

	public void AddBlackFade()
	{
		//TODO
	}
}