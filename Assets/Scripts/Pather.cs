using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//public enum Direction { NORTH, SOUTH, EAST, WEST };

public class Pather {

	private static Unit unit;

	public static Vector2 center;
	private static OldNode[,] nodes;
	private static Queue<OldNode> queue = new Queue<OldNode>();
	private static Vector2[] directions = new Vector2[4] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	public static List<Vector2> GetCoordsToMoveHighlight(Unit u)
	{
		unit = u;
		SetUpNodes(unit.xy, unit.owner, unit.grouping, false);
		return GetMoveCoordsFromNodes();
	}

	public static List<Vector2> GetAIMovePositions(Unit unit, bool forAttack)
	{
		List<Vector2> movePositions = new List<Vector2>();
		if (unit.behaviour == Behaviour.hold || 
		   (unit.behaviour == Behaviour.defend && !forAttack) ||
		   (unit.grouping == UnitGroup.artillery && forAttack))
		{
			GetCoordsToMoveHighlight(unit);
			movePositions.Add(unit.xy);
		} else {
			movePositions = Pather.GetCoordsToMoveHighlight(unit);
		}
		return movePositions;
	}

	public static void SetUpNodes(Vector2 c, int unitOwner, UnitGroup grouping, bool forFloodFill)
	{
		center = c;
		Tile[,] tiles = GridManager.GetTiles();
		Dictionary<Vector2,Unit> enemyUnits = GridManager.GetUnitsForOwner(BattleManager.GetPlayerIndexAfter(unitOwner));
		nodes = new OldNode[ GridManager.Width(), GridManager.Height() ];
		for (int x = 0; x < GridManager.Width(); x++)
		{
			for (int y = 0; y < GridManager.Height(); y++)
			{
				Vector2 position = new Vector2(x,y);
				int moveCost;
				if (enemyUnits.ContainsKey(position) && !forFloodFill)
				{
					moveCost = -1;
				} else {
					moveCost = tiles[x,y].moveCosts[(int)grouping];
				}
				nodes[x,y] = new OldNode(position, moveCost);
			}
		}
		nodes[(int)center.x,(int)center.y].pathCost = 0;
	}

	public static List<Vector2> GetMoveCoordsFromNodes()
	{
		List<Vector2> coords = new List<Vector2>();
		coords.Add(unit.xy);
		Dictionary<Vector2,Unit> friendlyUnits = GridManager.GetUnitsForOwner(unit.owner);
		queue.Enqueue(nodes[(int)center.x,(int)center.y]);
		while (queue.Count > 0)
		{
			OldNode u = queue.Dequeue();
			foreach (Vector2 direction in directions)
			{
				int vx = (int)(u.position.x + direction.x);
				int vy = (int)(u.position.y + direction.y);
				if (NodeInBounds(vx,vy))
				{
					OldNode v = nodes[vx, vy];
					if (v.moveCost > 0)
					{
						int newPathCost = u.pathCost + v.moveCost;
						if (newPathCost < v.pathCost && newPathCost <= unit.movePoints)
						{
							v.pathCost = newPathCost;
							v.trace = direction;
							if (!coords.Contains(v.position) && !friendlyUnits.ContainsKey(v.position)) coords.Add(v.position);
							queue.Enqueue(v);
						}
					}
				}
			}
		}
		return coords;
	}

	public static List<Vector2> GetMoveCoordsForFloodFill(Vector2 position, int movePoints)
	{
		List<Vector2> coords = new List<Vector2>();
		coords.Add(position);
		queue.Enqueue(nodes[(int)position.x,(int)position.y]);
		while (queue.Count > 0)
		{
			OldNode u = queue.Dequeue();
			foreach (Vector2 direction in directions)
			{
				int vx = (int)(u.position.x + direction.x);
				int vy = (int)(u.position.y + direction.y);
				if (NodeInBounds(vx,vy))
				{
					OldNode v = nodes[vx, vy];
					if (v.moveCost > 0)
					{
						int newPathCost = u.pathCost + v.moveCost;
						if (newPathCost < v.pathCost && newPathCost <= movePoints)
						{
							v.pathCost = newPathCost;
							v.trace = direction;
							if (!coords.Contains(v.position)) coords.Add(v.position);
							queue.Enqueue(v);
						}
					}
				}
			}
		}
		return coords;
	}

	private static bool NodeInBounds(int x, int y) {
		return x >= 0 && x < GridManager.Width() && y >= 0 && y < GridManager.Height();
	}

	public static List<Vector2> GetPathToPoint(Vector2 p)
	{
      List<Vector2> path = new List<Vector2>();
		int abortCounter = 0;
		while (p != center)
		{
         Debug.LogFormat("{0},{1}", p.x, p.y);
         OldNode target = nodes[(int)p.x,(int)p.y];
			path.Insert(0, target.trace);
			p -= target.trace;
			abortCounter++;
			if (abortCounter == 50) break;
		}
		return path;
	}

	public static int[,,] GetDistanceMap(Unit unit, Vector2 destination)
	{
		int[,,] distanceMap = new int[GridManager.Width(),GridManager.Height(),2];
		for (int x = 0; x < distanceMap.GetLength(0); x++)
		{
			for (int y = 0; y < distanceMap.GetLength(1); y++)
			{
				distanceMap[x,y,0] = 1000;
				distanceMap[x,y,1] = 1000;
			}
		}
		distanceMap[(int)destination.x,(int)destination.y,0] = 0;
		distanceMap[(int)destination.x,(int)destination.y,1] = 0;
		List<Vector2> positionsToCheck = new List<Vector2>();
		positionsToCheck.Add(destination);
		int turnsAway = 1;
		while (positionsToCheck.Count > 0)
		{
			List<Vector2> addList = new List<Vector2>();
			foreach (Vector2 positionToCheck in positionsToCheck)
			{
				Pather.SetUpNodes(positionToCheck, unit.owner, unit.grouping, true);
				List<Vector2> movePositions = Pather.GetMoveCoordsForFloodFill(positionToCheck, unit.movePoints);
				foreach (Vector2 movePosition in movePositions)
				{
					int pathCost = nodes[(int)movePosition.x,(int)movePosition.y].pathCost;
					if (turnsAway < distanceMap[(int)movePosition.x,(int)movePosition.y,0] || 
					   (turnsAway == distanceMap[(int)movePosition.x,(int)movePosition.y,0] && 
					   	pathCost < distanceMap[(int)movePosition.x,(int)movePosition.y,1]))
					{
						distanceMap[(int)movePosition.x,(int)movePosition.y,0] = turnsAway;
						distanceMap[(int)movePosition.x,(int)movePosition.y,1] = pathCost;
						addList.Add(movePosition);
					}
				}
			}
			positionsToCheck.Clear();
			foreach (Vector2 addVector in addList)
			{
				positionsToCheck.Add(addVector);
			}
			turnsAway++;
		}
		return distanceMap;
	}

	public static void LogDistanceMap(int[,,] map)
	{
		for (int y = map.GetLength(1) - 1; y >= 0; y--)
		{
			string row = "{ ";
			for (int x = 0; x < map.GetLength(0); x++)
			{
				string sT = (map[x,y,0] == 1000) ? "-" : map[x,y,0].ToString();
				string sP = (map[x,y,1] == 1000) ? "-" : map[x,y,1].ToString();
				row = row + "(" + sT + "." + sP + ") ";
			}
			row = row + " }";
			Debug.Log(row);
		}
	}
}


public class OldNode {

	public Vector2 position = new Vector2();
	public int moveCost;
	public int pathCost = 1000;
	public Vector2 trace;

	public OldNode(Vector2 position, int moveCost) {
		this.position = position;
		this.moveCost = moveCost;
	}
}
