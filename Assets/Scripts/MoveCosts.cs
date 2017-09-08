using System;
using System.Collections.Generic;

public static class MoveCosts {

   public static Dictionary<TileType,int> Infantry {
      get { return infantry;}
   }

   public static Dictionary<TileType,int> Cavalry {
      get { return cavalry;}
   }

   public static Dictionary<TileType,int> Artillery {
      get { return artillery;}
   }

   public static Dictionary<TileType,int> Flying {
      get { return flying;}
   }

   private static Dictionary<TileType,int> infantry = new Dictionary<TileType,int>() {
      {TileType.town, 1},
      {TileType.barracks, 1},
      {TileType.castle, 1},
      {TileType.road, 1},
      {TileType.plain, 1},
      {TileType.forest, 1},
      {TileType.river, 2},
      {TileType.mountain, 2},
      {TileType.sea, 1000}
   };
     
   private static Dictionary<TileType,int> cavalry = new Dictionary<TileType,int>() {
      {TileType.town, 1},
      {TileType.barracks, 1},
      {TileType.castle, 1},
      {TileType.road, 1},
      {TileType.plain, 1},
      {TileType.forest, 2},
      {TileType.river, 3},
      {TileType.mountain, 1000},
      {TileType.sea, 1000}
   };

   private static Dictionary<TileType,int> artillery = new Dictionary<TileType,int>() {
      {TileType.town, 1},
      {TileType.barracks, 1},
      {TileType.castle, 1},
      {TileType.road, 1},
      {TileType.plain, 2},
      {TileType.forest, 3},
      {TileType.river, 1000},
      {TileType.mountain, 1000},
      {TileType.sea, 1000}
   };

   private static Dictionary<TileType,int> flying = new Dictionary<TileType,int>() {
      {TileType.town, 1},
      {TileType.barracks, 1},
      {TileType.castle, 1},
      {TileType.road, 1},
      {TileType.plain, 1},
      {TileType.forest, 1},
      {TileType.river, 1},
      {TileType.mountain, 1},
      {TileType.sea, 1}
   };
}