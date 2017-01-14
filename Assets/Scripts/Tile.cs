using UnityEngine;
using System.Collections;


public class Tile : MonoBehaviour {

	public enum TileType {
		road, plain, forest, mountain, river, sea, 
		town, castle, barracks
	};

	public TileType type;
	public int[] moveCosts = new int[4];
	public int defenseBonus;


	void OnMouseDown() 
	{
		// notify InputManager
	}
	
}
