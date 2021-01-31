using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
	}


	public void ActivateSleepMode(Player player)
	{
		StartCoroutine(SleepModeCoroutine(player));
	}

	private IEnumerator SleepModeCoroutine(Player player)
	{
		player.controlEnabled = false;
		player.Sleep();
		float maxRange = player.roverEnergy.GetMaxRangeInPercent();

		var range = gameZone.bounds.size.magnitude * maxRange;

		var randomPos = GetRandomPosition(range);

		//todo cameraFade
		yield return new WaitForSeconds(1f);
		
		player.transform.position = randomPos;
		player.transform.rotation = Quaternion.identity;
		
		yield return new WaitForSeconds(1f);
		//todo cameraFade

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
}