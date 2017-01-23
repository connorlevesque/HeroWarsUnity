using UnityEngine;
using System.Collections;

public enum TileType { road, plain, forest, mountain, river, sea, castle, town, barracks };

public class Tile : MonoBehaviour {

	public TileType type;
	public int[] moveCosts = new int[4];
	public int defenseBonus = 0;
	public bool isBuilding = false;

	void OnMouseDown() 
	{
		// notify InputManager
	}
}
