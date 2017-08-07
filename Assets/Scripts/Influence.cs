using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Influence {

	public static float[,] influenceMap;
	public static float[,] oneTurnInfluenceMap;
	public static float[,] tensionMap;
	public static List<Vector2> maxTensionPoints = new List<Vector2>();
	public static float maxTension = 1f;

	public static void SetUpMaps()
	{
		influenceMap = new float[GridManager.Width(),GridManager.Height()];
		oneTurnInfluenceMap = new float[GridManager.Width(),GridManager.Height()];
		tensionMap = new float[GridManager.Width(),GridManager.Height()];
		maxTensionPoints.Clear();
		Dictionary<Vector2,Unit> units = GridManager.GetUnits();
		foreach (Unit unit in units.Values)
		{
			float[,] oneTurnUnitInfluence = new float[GridManager.Width(),GridManager.Height()];
			float[,] unitInfluenceMap = GetUnitInfluence(unit, ref oneTurnUnitInfluence);
			for (int x = 0; x < unitInfluenceMap.GetLength(0); x++)
			{
				for (int y = 0; y < unitInfluenceMap.GetLength(1); y++)
				{
					float influence = unitInfluenceMap[x,y];
					float oneTurnInfluence = oneTurnUnitInfluence[x,y];
					if (tensionMap[x,y] == 0) tensionMap[x,y] = 1;
					tensionMap[x,y] *= influence;
					if (unit.owner != BattleManager.GetCurrentPlayerIndex()) 
					{
						influence *= -1;
						oneTurnInfluence *= -1;
					}
					influenceMap[x,y] += influence;
					oneTurnInfluenceMap[x,y] += oneTurnInfluence;
				}
			}
		}
		maxTension = 1f;
		for (int x = 0; x < tensionMap.GetLength(0); x++)
		{
			for (int y = 0; y < tensionMap.GetLength(1); y++)
			{
				if (tensionMap[x,y] > maxTension)
				{
					maxTension = tensionMap[x,y];
					maxTensionPoints.Clear();
					maxTensionPoints.Add(new Vector2((float)x,(float)y));
				} else if (tensionMap[x,y] == maxTension) {
					maxTensionPoints.Add(new Vector2((float)x,(float)y));
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
		// 	List<Vector2> attackPositions = GridManager.GetCoordsToAttackHighlight(unit.xy, unit.range);
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

	public static void LogTensionPoints()
	{
		string str = "";
		foreach (Vector2 tensionPoint in maxTensionPoints)
		{
			str += "(" + tensionPoint.x.ToString() + "," + tensionPoint.y.ToString() + ") ";
		}
		Debug.Log(str);
	}

	public static void LogMap(float[,] map, bool isTension)
	{
		for (int y = map.GetLength(1) - 1; y >= 0; y--)
		{
			string row = "( ";
			for (int x = 0; x < map.GetLength(0); x++)
			{
				float content = map[x,y];
				if (isTension) content /= maxTension;
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

}
