using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InfluenceTwo {

	public static float[,] influenceMap;
	public static float[,] oneTurnInfluenceMap;
	public static float[,] enemyOneTurnInfluenceMap;
	public static List<Vector2> maxEnemyOneTurnInfluencePoints = new List<Vector2>();

	public static void SetUpMaps()
	{
		// influenceMap = new float[GridManager.Width(),GridManager.Height()];
		oneTurnInfluenceMap = new float[GridManager.Width(),GridManager.Height()];
		enemyOneTurnInfluenceMap = new float[GridManager.Width(),GridManager.Height()];
		Vector2 castleLocation = GridManager.GetCastleLocationForOwner(BattleManager.GetNextPlayerIndex());
		if (castleLocation != new Vector2(-100,-100))
		{
			enemyOneTurnInfluenceMap[(int)castleLocation.x,(int)castleLocation.y] += 50;
		}	
		Dictionary<Vector2,Unit> units = GridManager.GetUnits();
		foreach (Unit unit in units.Values)
		{
			float[,] oneTurnUnitInfluence = GetUnitOneTurnInfluence(unit);
			//float[,] unitInfluenceMap = GetUnitInfluence(unit);
			for (int x = 0; x < GridManager.Width(); x++)
			{
				for (int y = 0; y < GridManager.Height(); y++)
				{
					// float influence = unitInfluenceMap[x,y];
					float oneTurnInfluence = oneTurnUnitInfluence[x,y];
					float enemyOneTurnInfluence = oneTurnUnitInfluence[x,y];
					if (unit.owner != BattleManager.GetCurrentPlayerIndex()) 
					{
						// influence *= -1;
						oneTurnInfluence *= -1;
					} else {
						enemyOneTurnInfluence = 0;
					}
					// influenceMap[x,y] += influence;
					oneTurnInfluenceMap[x,y] += oneTurnInfluence;
					enemyOneTurnInfluenceMap[x,y] += enemyOneTurnInfluence;
				}
			}
		}
		float maxEnemyOneTurnInfluence = 0;
		for (int x = 0; x < enemyOneTurnInfluenceMap.GetLength(0); x++)
		{
			for (int y = 0; y < enemyOneTurnInfluenceMap.GetLength(1); y++)
			{
				if (enemyOneTurnInfluenceMap[x,y] > maxEnemyOneTurnInfluence)
				{
					maxEnemyOneTurnInfluence = enemyOneTurnInfluenceMap[x,y];
					maxEnemyOneTurnInfluencePoints.Clear();
					maxEnemyOneTurnInfluencePoints.Add(new Vector2(x,y));
				} else if (enemyOneTurnInfluenceMap[x,y] == maxEnemyOneTurnInfluence) {
					maxEnemyOneTurnInfluencePoints.Add(new Vector2(x,y));
				}
			}
		}
	}

	public static float[,] GetUnitInfluence(Unit unit, ref float[,] oneTurnUnitInfluence)
	{
		float[,] unitInfluenceMap = new float[GridManager.Width(),GridManager.Height()];
		int turnsAway = 1;
		// bool isArtillery = unit.grouping == UnitGroup.artillery;
		// if (isArtillery)
		// {
		// 	List<Vector2> attackPositions = GridManager.GetCoordsToAttackHighlight(unit.transform.position, unit.range);
		// 	foreach (Vector2 attackPosition in attackPositions)
		// 	{
		// 		float influence = (float)unit.GetPower() / (float)Math.Pow(2 , Math.Pow(turnsAway, 2));
		// 		if (influence > unitInfluenceMap[(int)attackPosition.x,(int)attackPosition.y])
		// 		{
		// 			unitInfluenceMap[(int)attackPosition.x,(int)attackPosition.y] = influence;
		// 			if (turnsAway == 1)
		// 			{
		// 				oneTurnUnitInfluence[(int)attackPosition.x,(int)attackPosition.y] = influence;
		// 			}
		// 		}
		// 	}
		// 	turnsAway++;
		// }
		List<Vector2> positionsToCheck = new List<Vector2>();
		positionsToCheck = Pather.GetAIMovePositions(unit, true);
		while (positionsToCheck.Count > 0)
		{
			List<Vector2> addList = new List<Vector2>();
			foreach (Vector2 positionToCheck in positionsToCheck)
			{
				Pather.SetUpNodes(positionToCheck, unit.owner, unit.grouping, true);
				List<Vector2> movePositions = Pather.GetMoveCoordsForFloodFill(positionToCheck, unit.movePoints);
				foreach (Vector2 movePosition in movePositions)
				{
					List<Vector2> attackPositions = GridManager.GetCoordsToAttackHighlight(movePosition, unit.range);
					foreach (Vector2 attackPosition in attackPositions)
					{
						float influence = (float)unit.GetPower() / (float)Math.Pow(2 , Math.Pow(turnsAway, 2));
						if (influence > unitInfluenceMap[(int)attackPosition.x,(int)attackPosition.y])
						{
							unitInfluenceMap[(int)attackPosition.x,(int)attackPosition.y] = influence;
							if (turnsAway == 1)
							{
								oneTurnUnitInfluence[(int)attackPosition.x,(int)attackPosition.y] = influence;
							}
							addList.Add(movePosition);
						}
					}
				}
			}
			positionsToCheck.Clear();
			if (unit.behaviour != Behaviour.defend)
			{
				foreach (Vector2 addVector in addList)
				{
					positionsToCheck.Add(addVector);
				}
			}
			//Debug.LogFormat("There are {0} movePositions at {1} turnsAway", positionsToCheck.Count, turnsAway);
			turnsAway++;
		}
		return unitInfluenceMap;
	}

	public static float[,] GetUnitOneTurnInfluence(Unit unit)
	{
		float[,] oneTurnUnitInfluence = new float[GridManager.Width(),GridManager.Height()];
		foreach (Vector2 attackPoint in InputManager.GetRangeStateCoords(unit))
		{
			oneTurnUnitInfluence[(int)attackPoint.x,(int)attackPoint.y] = (float)unit.GetPower();
		}
		return oneTurnUnitInfluence;
	}

	public static void LogMap(float[,] map)
	{
		for (int y = map.GetLength(1) - 1; y >= 0; y--)
		{
			string row = "( ";
			for (int x = 0; x < map.GetLength(0); x++)
			{
				float content = map[x,y];
				row = row + content.ToString() + ", ";
			}
			row = row + " )";
			Debug.LogFormat("{0}", row);
		}
	}

	public static void LogMap(int[,] map)
	{
		for (int y = map.GetLength(1) - 1; y >= 0; y--)
		{
			string row = "( ";
			for (int x = 0; x < map.GetLength(0); x++)
			{
				row = row + map[x,y].ToString() + ", ";
			}
			row = row + " )";
			Debug.LogFormat("{0}", row);
		}
	}

	public static void LogPoints(List<Vector2> points)
	{
		string str = "";
		foreach (Vector2 point in points)
		{
			str = str + "(" + point.x.ToString() + "," + point.y.ToString() + ") ";
		}
		Debug.Log(str);
	}

}
