using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
	[SerializeField] [Range(0, 1f)] private float maxHeigth = 0.01f;
	[SerializeField] private Terrain topLeftTerrain;
	private List<Terrain> terrains;
	private int heigthMapWidth;
	private int heightmapHeight;

	[SerializeField] [MinMaxSlider(0f, 100f)]
	private Vector2 circleDimensions;

	[SerializeField] [MinMaxSlider(-1f, 1f)]
	private Vector2 heigthCircleDelta;

	[SerializeField] [MinMaxSlider(0, 100)]
	private Vector2 circleDensity;

	[SerializeField] private AnimationCurve[] craterCurves;
	[SerializeField] private int x, y;

	private void Awake()
	{
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
		terrains = GetComponentsInChildren<Terrain>().ToList();

		int minX = (int) (terrains.Min(x => x.transform.position.x) / 1000);
		int minY = (int) (terrains.Min(x => x.transform.position.z) / 1000);
		int maxX = (int) (terrains.Max(x => x.transform.position.x) / 1000);
		int maxY = (int) (terrains.Max(x => x.transform.position.z) / 1000);

		int hori = (int) (maxX - minX) + 1;
		int verti = (int) (maxY - minY) + 1;

		var terrainMatrix = new Terrain[hori, verti];
		int xx = 0;
		int yy = 0;
		for (int i = minY; i <= maxY; i++)
		{
			for (int j = minX; j <= maxX; j++)
			{
				terrainMatrix[xx, yy] = terrains.FirstOrDefault(x => Math.Abs(x.transform.position.x - i* 1000) < 10 && Math.Abs(x.transform.position.z - j * 1000) < 10);
				xx++;
			}

			yy++;
			xx = 0;
		}


		int size = terrains[0].terrainData.heightmapResolution;
		float[,] heights = new float[size * hori, size * verti];

		for (int i = 0; i < size * hori; i++)
		{
			for (int j = 0; j < size * verti; j++)
			{
				heights[i, j] = maxHeigth / 2;
			}
		}

		int craters = Random.Range(Mathf.RoundToInt(circleDensity.x * Mathf.Sqrt(terrains.Count)), Mathf.RoundToInt(circleDensity.y * Mathf.Sqrt(terrains.Count)));
		for (int i = 0; i < craters; i++)
		{
			var radius = Random.Range(circleDimensions.x, circleDimensions.y);
			var heigth = Random.Range(heigthCircleDelta.x, heigthCircleDelta.y);
			var x = Random.Range(0, size * hori);
			var y = Random.Range(0, size * verti);
			GenerateMountain(heights, size * hori, size * verti, new Vector2(x, y), radius, heigth);
		}


		for (int i = 0; i < hori; i++)
		{
			for (int j = 0; j < verti; j++)
			{
				var offsetY = size * i;
				var offsetX = size * j;

				var h = new float[size, size];
				for (int a = 0; a < size; a++)
				{
					for (int b = 0; b < size; b++)
					{
						h[b, a] = heights[offsetY + b, offsetX + a];
					}
				}

				terrainMatrix[i, j].terrainData.SetHeights(0, 0, h);
			}
		}
	}

	private void GenerateMountain(float[,] heights, int sizeX, int sizeY, Vector2 center, float radius, float heigth)
	{
		var craterCurve = craterCurves[Random.Range(0, craterCurves.Length)];
		Debug.Log($"Generated circle @({center.x}, {center.y}), radius = {radius}, heigth = {heigth}%");


		for (int x = Mathf.Max(0, Mathf.FloorToInt(center.x - radius)); x < Mathf.Min(sizeX,Mathf.CeilToInt(center.x + radius)); x++)
		{
			for (int y = Mathf.Max(0, Mathf.FloorToInt(center.y - radius)); y < Mathf.Min(sizeY, Mathf.CeilToInt(center.y + radius)); y++)
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
		//terrain = this.GetComponentInChildren<Terrain>();

		//int size = terrain.terrainData.heightmapResolution;
		//var data = terrain.terrainData;
		//float[,] heights = data.GetHeights(0, 0, size, size);

		//for (int i = 1; i < size - 1; i++)
		//{
		//	for (int j = 1; j < size - 1; j++)
		//	{
		//		heights[i, j] = (heights[i - 1, j - 1] + heights[i + 1, j + 1] + heights[i - 1, j + 1] + heights[i + 1, j - 1]) / 4;
		//	}
		//}


		//data.SetHeights(0, 0, heights);
	}
}