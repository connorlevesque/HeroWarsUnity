using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComputerOpponent {

	private static List<Unit> units = new List<Unit>();

	public static void Run()
	{
		Debug.Log("Running ComputerOpponent");
		OrderUnitList(false);

	}

	public static void CheckAttacks()
	{
		foreach (Unit unit in units)
		{
			foreach (Unit target in GetAttackTargets())
			{
				Debug.LogFormat("{0} can attack {1}", unit.type.ToString(), target.type.ToString());
			}
		}
	}

	public static List<Unit> GetAttackTargets()
	{
		List<Unit> targets = new List<Unit>();
		
	}

	public static void OrderUnitList(bool increasingRange)
	{
		units.Clear();
		Dictionary<Vector2,Unit> unitDict = GridManager.GetFriendlyUnits();
		foreach (Unit unit in unitDict.Values)
		{
			if (unit.activated) 
			{
				int index = units.Count;
				for (int i = 0; i < units.Count; i++)
				{
					Unit orderedUnit = units[i];
					if (increasingRange)
					{
						if (unit.range[1] < orderedUnit.range[1]) 
						{
							index = i; break;
						} 
					} else {
						if (unit.range[1] > orderedUnit.range[1]) 
						{
							index = i; break;
						} 
					}
					if ((unit.range[1] == orderedUnit.range[1]) && 
								unit.GetPower() > orderedUnit.GetPower()) {
						index = i; break;
					}
				}
				units.Insert(index, unit);
			}
		}
	}

	public static void LogUnitList()
	{
		Debug.Log("Logging unit list");
		foreach (Unit unit in units)
		{
			Debug.LogFormat("{0} {1}", unit.type.ToString(), unit.GetPower());
		}
	}

}
