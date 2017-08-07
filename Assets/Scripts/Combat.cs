using UnityEngine;
using System.Collections;
using System;

public class Combat {

	public static Unit[] CalculateCombatForUnits(Unit attacker, Unit defender)
	{
		Unit[] units = new Unit[2] {attacker, defender};
		if (	   CanAttack(attacker, defender)) units[1] = Attack(attacker, defender); // attacker strikes defender
		if (CanCounterAttack(defender, attacker)) units[0] = Attack(defender, attacker); // if they can, defender strikes attacker
		return units;
	}

	public static bool CanAttack(Unit a, Unit b)
	{
		if (a.health > 0 && a.owner != b.owner)
		{
			int dx = (int)Mathf.Abs(a.x - b.x);
			int dy = (int)Mathf.Abs(a.y - b.y);
			if (a.range[0] <= dx + dy && dx + dy <= a.range[1])
			{
				return true;
			}
		}
		return false;
	}

	public static bool CanCounterAttack(Unit a, Unit b)
	{
		if (a.health > 0 && a.owner != b.owner && a.grouping != UnitGroup.artillery)
		{
			int dx = (int)Mathf.Abs(a.x - b.x);
			int dy = (int)Mathf.Abs(a.y - b.y);
			if (a.range[0] <= dx + dy && dx + dy <= a.range[1])
			{
				return true;
			}
		}
		return false;
	}

	public static Unit Attack(Unit a, Unit b)
	{
		int finalDamage = GetFinalDamage(a,b,true);
		b.ChangeHealth(finalDamage * -1);
		return b;
	}

	public static int GetFinalDamage(Unit a, Unit b, bool useRandom)
	{
		float h = a.health / 100f;
		// int d = a.damage;
		// float p = a.pen;
		// if (b.grouping == a.bonusCondition) 
		// {
		// 	d += a.bonusDamage;
		// 	p += a.bonusPen;
		// }
		// float arm = Math.Max(b.armor - p, 0);
		float defenseBonus = GetDefenseBonus(b);
		float t = (100 - (defenseBonus * b.health / 10f)) / 100f;
		float r = (useRandom) ? UnityEngine.Random.Range(-2f, 2f) : 0;
		// return (int)Math.Round(r + h * (d / Math.Pow(2, b.armor / Math.Pow(2, p))));
		return Math.Max((int)Math.Round(r + t * h * (DamageTable.GetBaseDamage(a.type, b.type))), 1);
	}

	public static int GetDefenseBonus(Unit unit)
	{
		Tile tile = GridManager.GetTile(unit.xy);
		int defenseBonus = tile.defenseBonus;
		if (tile.isBuilding)
		{
			Building building = (Building)tile;
			if (building.owner == unit.owner)
			{
				defenseBonus++;
			}
		}
		return defenseBonus;
	}

}
