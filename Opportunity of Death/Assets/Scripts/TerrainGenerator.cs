using System.Collections;
using Sirenix.OdinInspector;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
	[SerializeField] [Range(0, 0.2f)] private float maxHeigth = 0.01f;
	private Terrain terrain;
	private int heigthMapWidth;
	private int heightmapHeight;

	[SerializeField] [MinMaxSlider(0f, 100f)]
	private Vector2 circleDimensions;

	[SerializeField] [MinMaxSlider(-1f, 1f)]
	private Vector2 heigthCircleDelta;

	[SerializeField] [MinMaxSlider(0,100)]
	private Vector2 circleNumberRange;

	[SerializeField] private AnimationCurve[] craterCurves;

	private void Awake()
	{
		terrain = this.GetComponentInChildren<Terrain>();
	}

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	[Button]
	void Generate()
	{
		terrain = this.GetComponentInChildren<Terrain>();

		int size = terrain.terrainData.heightmapResolution;

		var data = terrain.terrainData;
		float[,] heights = new float[size, size];
		WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				heights[i, j] = maxHeigth / 2;
			}
		}

		data.SetHeights(0, 0, heights);

		int craters = Random.Range(Mathf.RoundToInt(circleNumberRange.x), Mathf.RoundToInt(circleNumberRange.y));
		for (int i = 0; i < craters; i++)
		{
			var radius = Random.Range(circleDimensions.x, circleDimensions.y);
			var heigth = Random.Range(heigthCircleDelta.x, heigthCircleDelta.y);
			var x = Random.Range(0, size);
			var y = Random.Range(0, size);
			GenerateMountain(heights, size, new Vector2(x, y), radius, heigth);
		}

		data.SetHeights(0, 0, heights);
	}

	private void GenerateMountain(float[,] heights, int size, Vector2 center, float radius, float heigth)
	{
		var craterCurve = craterCurves[Random.Range(0, craterCurves.Length)];
		Debug.Log($"Generated circle @({center.x}, {center.y}), radius = {radius}, heigth = {heigth}%");
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				var res = Mathf.Pow(x - center.x, 2) + Mathf.Pow(y - center.y, 2);

				if (res <= Mathf.Pow(radius, 2))
				{
					heights[x, y] += craterCurve.Evaluate(Mathf.Sqrt(res) / radius) * heigth * maxHeigth;
				}
			}
		}
	}

	[Button]
	private void Smooth()
	{
		terrain = this.GetComponentInChildren<Terrain>();

		int size = terrain.terrainData.heightmapResolution;
		var data = terrain.terrainData;
		float[,] heights = data.GetHeights(0, 0, size, size);

		for (int i = 1; i < size - 1; i++)
		{
			for (int j = 1; j < size - 1; j++)
			{
				heights[i, j] = (heights[i - 1, j - 1] + heights[i + 1, j + 1] + heights[i - 1, j + 1] + heights[i + 1, j - 1]) / 4;
			}
		}


		data.SetHeights(0, 0, heights);
	}
}