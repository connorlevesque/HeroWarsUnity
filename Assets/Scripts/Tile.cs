using UnityEngine;
using System;
using System.Collections;

public enum TileType { road, plain, forest, mountain, river, sea, castle, town, barracks };

public class Tile : MonoBehaviour {

   public Vector2 xy;
      public int x {get{return (int)xy.x;}}
      public int y {get{return (int)xy.y;}}

	public TileType type;
	public int[] moveCosts = new int[4];
	public int defenseBonus = 0;
	public bool isBuilding = false;

   protected void Start() {
      Register();
   }

   private void Register() {
      int x = (int)Math.Round(transform.position.x);
      int y = (int)Math.Round(transform.position.y);
      xy = new Vector2(x,y);
      transform.position = xy;
      GridManager.RegisterTile(this);
   }
}
