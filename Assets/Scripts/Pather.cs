using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//public enum Direction { NORTH, SOUTH, EAST, WEST };


public class Pather {

	private static Unit unit;

	public static Vector2 center;
	private static Node[,] nodes;
	private static Queue<Node> queue = new Queue<Node>();
	private static Vector2[] directions = new Vector2[4] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	public static List<Vector2> GetCoordsToMoveHighlight(Unit u)
	{
		unit = u;
		SetUpNodes(unit.transform.position, unit.grouping);
		return GetMoveCoordsFromNodes();
	}

	public static void SetUpNodes(Vector2 c, UnitGroup grouping)
	{
		center = c;
		Tile[,] tiles = GridManager.GetTiles();
		Dictionary<Vector2,Unit> enemyUnits = GridManager.GetEnemyUnits();
		nodes = new Node[ GridManager.Width(), GridManager.Height() ];
		for (int x = 0; x < GridManager.Width(); x++)
		{
			for (int y = 0; y < GridManager.Height(); y++)
			{
				Vector2 position = new Vector2(x,y);
				int moveCost;
				if (enemyUnits.ContainsKey(position))
				{
					moveCost = -1;
				} else {
					moveCost = tiles[x,y].moveCosts[(int)grouping];
				}
				nodes[x,y] = new Node(position, moveCost);
			}
		}
		nodes[(int)center.x,(int)center.y].pathCost = 0;
	}

	public static List<Vector2> GetMoveCoordsFromNodes()
	{
		List<Vector2> coords = new List<Vector2>();
		Dictionary<Vector2,Unit> friendlyUnits = GridManager.GetFriendlyUnits();
		queue.Enqueue(nodes[(int)center.x,(int)center.y]);
		while (queue.Count > 0)
		{
			Node u = queue.Dequeue();
			foreach (Vector2 direction in directions)
			{
				int vx = (int)(u.position.x + direction.x);
				int vy = (int)(u.position.y + direction.y);
				if (NodeInBounds(vx,vy))
				{
					Node v = nodes[vx, vy];
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
		Dictionary<Vector2,Unit> friendlyUnits = GridManager.GetFriendlyUnits();
		queue.Enqueue(nodes[(int)position.x,(int)position.y]);
		while (queue.Count > 0)
		{
			Node u = queue.Dequeue();
			foreach (Vector2 direction in directions)
			{
				int vx = (int)(u.position.x + direction.x);
				int vy = (int)(u.position.y + direction.y);
				if (NodeInBounds(vx,vy))
				{
					Node v = nodes[vx, vy];
					if (v.moveCost > 0)
					{
						int newPathCost = u.pathCost + v.moveCost;
						if (newPathCost < v.pathCost && newPathCost <= movePoints)
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

	private static bool NodeInBounds(int x, int y)
	{
		if ((x >= 0) &&
			(x < GridManager.Width()) &&
			(y >= 0) &&
			(y < GridManager.Height()))
		{
			return true;
		} else {
			return false;
		}
	}

	public static List<Vector2> GetPathToPoint(Vector2 p)
	{
		List<Vector2> path = new List<Vector2>();
		while (p != center)
		{
			Node target = nodes[(int)p.x,(int)p.y];
			path.Insert(0, target.trace);
			p -= target.trace;
		}
		return path;
	}

	public static int[,] GetDistanceMap(Unit unit, Vector2 destination)
	{
		int[,] distanceMap = new int[GridManager.Width(),GridManager.Height()];
		for (int x = 0; x < distanceMap.GetLength(0); x++)
		{
			for (int y = 0; y < distanceMap.GetLength(1); y++)
			{
				distanceMap[x,y] = 1000;
			}
		}
		distanceMap[(int)destination.x,(int)destination.y] = 0;
		Pather.SetUpNodes(destination, unit.grouping);
		List<Vector2> positionsToCheck = Pather.GetMoveCoordsForFloodFill(destination, unit.movePoints);
		int turnsAway = 1;
		while (positionsToCheck.Count > 0)
		{
			List<Vector2> addList = new List<Vector2>();
			foreach (Vector2 positionToCheck in positionsToCheck)
			{
				Pather.SetUpNodes(positionToCheck, unit.grouping);
				List<Vector2> movePositions = Pather.GetMoveCoordsForFloodFill(positionToCheck, unit.movePoints);
				foreach (Vector2 movePosition in movePositions)
				{
					if (turnsAway < distanceMap[(int)movePosition.x,(int)movePosition.y])
					{
						distanceMap[(int)movePosition.x,(int)movePosition.y] = turnsAway;
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
}


public class Node {

	public Vector2 position = new Vector2();
	public int moveCost;
	public int pathCost = 100;
	public Vector2 trace;

	public Node(Vector2 p, int mC)
	{
		position = p;
		moveCost = mC;
	}

}
