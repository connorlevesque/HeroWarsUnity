using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Influence {

	public static float[,] influenceMap;
	public static float[,] tensionMap;
	public static List<Vector2> maxTensionPoints = new List<Vector2>();

	public static void SetUpMaps()
	{
		influenceMap = new float[GridManager.Width(),GridManager.Height()];
		tensionMap = new float[GridManager.Width(),GridManager.Height()];
		maxTensionPoints.Clear();
		Dictionary<Vector2,Unit> units = GridManager.GetUnits();
		foreach (Unit unit in units.Values)
		{
			float[,] unitInfluenceMap = GetUnitInfluence(unit);
			for (int x = 0; x < unitInfluenceMap.GetLength(0); x++)
			{
				for (int y = 0; y < unitInfluenceMap.GetLength(1); y++)
				{
					float influence = unitInfluenceMap[x,y];
					if (tensionMap[x,y] == 0) tensionMap[x,y] = 1;
					tensionMap[x,y] *= influence;
					if (unit.owner != BattleManager.GetCurrentPlayerIndex()) influence *= -1;
					influenceMap[x,y] += influence;
				}
			}
		}
		float maxTension = 1f;
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
				tensionMap[x,y] /= maxTension;
			}
		}
		Debug.LogFormat("maxTension = {0}", maxTension);
	}

	public static float[,] GetUnitInfluence(Unit unit)
	{
		float[,] unitInfluenceMap = new float[GridManager.Width(),GridManager.Height()];
		List<Vector2> positionsToCheck = new List<Vector2>();
		positionsToCheck = Pather.GetCoordsToMoveHighlight(unit);
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
					List<Vector2> attackPositions = GridManager.GetCoordsToAttackHighlight(movePosition, unit.range);
					foreach (Vector2 attackPosition in attackPositions)
					{
						float influence = (float)unit.GetPower() / (float)Math.Pow(2 , Math.Pow(turnsAway, 2));
						if (influence > unitInfluenceMap[(int)movePosition.x,(int)movePosition.y])
						{
							unitInfluenceMap[(int)movePosition.x,(int)movePosition.y] = influence;
							addList.Add(movePosition);
						}
					}
				}
			}
			positionsToCheck.Clear();
			foreach (Vector2 addVector in addList)
			{
				positionsToCheck.Add(addVector);
			}
			//Debug.LogFormat("There are {0} movePositions at {1} turnsAway", positionsToCheck.Count, turnsAway);
			turnsAway++;
		}
		return unitInfluenceMap;
	}

	public static void LogMap(float[,] map)
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
