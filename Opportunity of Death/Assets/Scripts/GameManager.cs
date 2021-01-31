using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	public static GameManager Instance => instance;

	[SerializeField] private Camera camera;
	[SerializeField] private Terrain[] terrains;
	[SerializeField] private Transform motherBase;
	[SerializeField] private Collider gameZone;
	[SerializeField] private LayerMask terrainLayerMask;

	[SerializeField] [MinMaxSlider(0f, 2f)]
	private Vector2 spawnDistanceRange = new Vector2(0.8f, 1.2f);

	[SerializeField] private RoverUi ui;

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

	public void Start()
	{
		StartCoroutine(InitialFade());
	
	}

	private IEnumerator InitialFade()
	{
		yield return new WaitForSeconds(1.5f);
		GameManager.Instance.ShowText("Opportunity, Mission Control here. Our communication dish is malfunctioning. Check it out!", 4f);
		ui.FadeOff(1f);
	}

	private bool firstSleep = true;
	public void ActivateSleepMode(Player player)
	{
		if (firstSleep)
		{
			firstSleep = false;
			player.GetComponent<DeeJay>().AmbientMusicStart();
		}	 
		StartCoroutine(SleepModeCoroutine(player));
	}

	private IEnumerator SleepModeCoroutine(Player player)
	{
		player.controlEnabled = false;
		player.Sleep();
		float maxRange = player.roverEnergy.GetMaxRangeInPercent();

		var range = gameZone.bounds.size.magnitude * maxRange;

		var randomPos = GetRandomPosition(range);

		ui.FadeOn(1f);
		yield return new WaitForSeconds(1f);

		player.transform.position = randomPos;
		player.transform.rotation = Quaternion.identity;

		yield return new WaitForSeconds(1.5f);

		ui.FadeOff(1f);
		yield return new WaitForSeconds(0.5f);
		player.controlEnabled = true;
		yield return new WaitForSeconds(1f);
	}

	public void AddGlitch()
	{
		StartCoroutine(StartGlitchEffect());
	}

	private IEnumerator StartGlitchEffect()
	{
		GlitchEffect glitchEffect = camera.GetComponent<GlitchEffect>();
		float value = 0;
		while (Math.Abs(value - 1) > 0.05)
		{
			value = Mathf.Lerp(value, 1, Time.deltaTime / 10);
			glitchEffect.intensity = value;
			glitchEffect.colorIntensity = value;
			glitchEffect.flipIntensity = value;

			yield return null;
		}
	}

	public void AddBlackFade()
	{
	}

	private Vector3 spawnPosition;

	[Button]
	private Vector3 GetRandomPosition(float range)
	{
		int i = 0;
		var zoneCenter = gameZone.bounds.center;
		var zoneBounds = gameZone.bounds.extents;
		while (true)
		{
			//var x = motherBase.position.x + Mathf.Sqrt(Random.Range(range * spawnDistanceRange.x, range * spawnDistanceRange.y));
			//var z = motherBase.position.y + Mathf.Sqrt(Random.Range(range * spawnDistanceRange.x, range * spawnDistanceRange.y));

			var x = Random.Range(zoneCenter.x - zoneBounds.x, zoneCenter.x + zoneBounds.x);
			var z = Random.Range(zoneCenter.z - zoneBounds.z, zoneCenter.x + zoneBounds.z);

			var r = new Ray(new Vector3(x, 600, z), Vector3.down);

			if (Physics.Raycast(r.origin, r.direction, out RaycastHit hit, 1000f))
			{
				bool inrange = (hit.point - motherBase.position).magnitude > range * spawnDistanceRange.x
							&& (hit.point - motherBase.position).magnitude < range * spawnDistanceRange.y;

				if (inrange && hit.collider is TerrainCollider tc)
				{
					spawnPosition = tc.ClosestPoint(hit.point) + Vector3.up * 10;
					return spawnPosition;
				}
			}

			if (++i > 100)
				return spawnPosition;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(spawnPosition, 50f);
		Gizmos.DrawLine(spawnPosition, motherBase.position);
	}

	public void ShowText(string text)
	{
		ui.ShowText(text);
	}

	public void ShowText(string text, float duration)
	{
		StartCoroutine(TextCoroutine(text, duration));
	}

	private IEnumerator TextCoroutine(string text, float duration)
	{
		ui.ShowText(text);
		yield return new WaitForSeconds(duration);
		ui.HideText();
	}


	public void HideText()
	{
		ui.HideText();
	}

	
}