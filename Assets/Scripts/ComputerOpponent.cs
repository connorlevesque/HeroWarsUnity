using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComputerOpponent : MonoBehaviour {

	private List<Unit> units = new List<Unit>();
	private List<Unit> unitsToRemove = new List<Unit>();
	private bool inSubCoroutine = false;
	private bool moving = false;

	public IEnumerator Run()
	{
		Debug.Log("Running ComputerOpponent");
		OrderUnitList(false);
		inSubCoroutine = true;
		StartCoroutine(CheckAttacks());
		while (inSubCoroutine) {
			yield return new WaitForSeconds(.1f);
		}
		InputManager.ChangeTurns();
	}

	public IEnumerator CheckAttacks()
	{
		foreach (Unit unit in units)
		{
			Vector2 attackPosition = new Vector2();
			Unit target = ChooseTarget(unit, ref attackPosition);
			if (target)
			{
				Debug.LogFormat("{0} {1} will attack {2} {3}", unit.type.ToString(), 
					unit.owner.ToString(), target.type.ToString(), target.owner.ToString());
				moving = true;
				GridManager.MoveUnitAlongPath(unit, attackPosition, Pather.GetPathToPoint(attackPosition), () => { 
					moving = false;
				});
				while (moving) {
					yield return new WaitForSeconds(.1f);
				}
				CompleteAttack(unit, target);
			}
		}
		RemoveUsedUnits();
		LogUnitList();
		inSubCoroutine = false;
	}

	public void CompleteAttack(Unit unit, Unit target)
	{
		Debug.LogFormat("CompleteAttack from {0} {1} to {2} {3}", unit.type.ToString(), unit.owner.ToString(),
			target.type.ToString(), target.owner.ToString());
		GridManager.CalculateAttack(unit, target);
		unit.Deactivate();
		unitsToRemove.Add(unit);
	}

	public void RemoveUsedUnits()
	{
		foreach (Unit unit in unitsToRemove)
		{
			units.Remove(unit);
		}
		unitsToRemove.Clear();
	}

	public Unit ChooseTarget(Unit unit, ref Vector2 attackPosition)
	{
		Vector2 startingPosition = unit.transform.position;
		Unit chosenTarget = null;
		float bestAttackValue = -100000;
		foreach (Vector2 position in GetMovePositions(unit, true))
		{
			GridManager.MoveUnit(startingPosition, position, () => {});
			foreach (Vector2 targetPosition in GridManager.GetCoordsToAttackHighlight(unit))
			{
				Unit target = GridManager.GetUnit(targetPosition);
				if (target)
				{
					if (target.owner != unit.owner)
					{
						int attackerStartingHealth = unit.health;
						int defenderStartingHealth = target.health;
						Unit[] outCome = Combat.CalculateCombatForUnits(unit, target);
						int damageDone = defenderStartingHealth - outCome[1].health;
						int damageTaken = attackerStartingHealth - outCome[0].health;
						bool enoughDamageDone = (damageTaken > 0) ? (damageDone > 10 || damageDone / damageTaken > 2f) : true;
						bool notTooMuchDamageTaken = damageTaken < 70;
						if (enoughDamageDone && notTooMuchDamageTaken)
						{
							float attackValue = damageDone * target.cost * target.powerConstant -
												damageTaken * unit.cost * unit.powerConstant;
							if ((attackValue > bestAttackValue) ||
								(attackValue == bestAttackValue && UnityEngine.Random.Range(0f, 1f) > .5f))
							{
								bestAttackValue = attackValue;
								chosenTarget = target;
								attackPosition = position;
							}
						}
						unit.ChangeHealth(damageTaken);
						target.ChangeHealth(damageDone);
					}
				}
			}
			GridManager.MoveUnit(position, startingPosition, () => {});
		}
		return chosenTarget;
	}

	public List<Vector2> GetMovePositions(Unit unit, bool forAttack)
	{
		List<Vector2> movePositions = new List<Vector2>();
		if (unit.behaviour == Behaviour.hold || (unit.behaviour == Behaviour.defend && !forAttack))
		{
			movePositions.Add(unit.transform.position);
		} else {
			movePositions = Pather.GetCoordsToMoveHighlight(unit);
		}
		return movePositions;
	}

	public void OrderUnitList(bool increasingRange)
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

	public void LogUnitList()
	{
		Debug.Log("Logging unit list");
		foreach (Unit unit in units)
		{
			Debug.LogFormat("{0} {1}", unit.type.ToString(), unit.GetPower());
		}
	}

}
