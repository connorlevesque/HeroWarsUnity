using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	public int width;
	public int height;
	public GameObject[] startingTiles;
	public GameObject[] startingUnits;

	private Tile[,] tiles;
	private Dictionary<Vector2,Unit> units = new Dictionary<Vector2,Unit>();

	// Use this for initialization
	void Start () {
		tiles = new Tile[width, height];
		ParseStartingTiles();
		ParseStartingUnits();
	}

	private void ParseStartingTiles()
	{
		int x;
		int y;
		foreach(GameObject element in startingTiles)
		{
			if (element.GetComponent<Tile>())
			{
				Tile tile = element.GetComponent<Tile>();
				x = (int)tile.transform.position.x;
				y = (int)tile.transform.position.y;
				tiles[x,y] = tile;
			}
		}
	}

	private void ParseStartingUnits()
	{
		int x;
		int y;
		foreach(GameObject element in startingUnits)
		{
			if (element.GetComponent<Unit>())
			{
				Unit unit = element.GetComponent<Unit>();
				x = (int)unit.transform.position.x;
				y = (int)unit.transform.position.y;
				units.Add(new Vector2(x,y), unit);
			}
		}
	}

}
