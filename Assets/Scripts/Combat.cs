using UnityEngine;
using System.Collections;

public class Combat {

	public static Unit[] CalculateCombatForUnits(Unit attacker, Unit defender)
	{
		Unit[] units = new Unit[2] {attacker, defender};
		if (canStrike(attacker, defender)) units[1] = Strike(attacker, defender); // attacker strikes defender
		if (canStrike(defender, attacker)) units[0] = Strike(defender, attacker); // if they can, defender strikes attacker
		return units;
	}

	public static bool canStrike(Unit a, Unit b)
	{
		if (a.health > 0 && a.grouping != UnitGroup.artillery)
		{
			int dx = (int)Mathf.Abs(a.transform.position.x - b.transform.position.x);
			int dy = (int)Mathf.Abs(a.transform.position.y - b.transform.position.y);
			if (a.range[0] <= dx + dy && dx + dy <= a.range[1])
			{
				return true;
			}
		}
		return false;
	}

	public static Unit Strike(Unit a, Unit b)
	{
		b.ChangeHealth(-50);
		return b;
	}
}
