using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComputerOpponent : MonoBehaviour {

	private List<Unit> units = new List<Unit>();
	private List<Unit> unitsToRemove = new List<Unit>();
	private bool inSubCoroutine = false;
	private bool moving = false;
	private bool attackFound = false;
	private List<Vector2> captureAssignments = new List<Vector2>();
	public int safeInfluence = 0;

	public IEnumerator Run()
	{
		Debug.Log("Running ComputerOpponent");
		// check for attacks
		OrderUnitList(false);
		inSubCoroutine = true;
		do {
			StartCoroutine(CheckAttacks());
			while (inSubCoroutine) {
				yield return new WaitForSeconds(.1f);
			}
		} while (attackFound);
		// check for moves
		OrderUnitList(true);
		inSubCoroutine = true;
		StartCoroutine(CheckMoves());
		while (inSubCoroutine) {
			yield return new WaitForSeconds(.1f);
		}
		// train units
		TrainUnits();
		// cleanup
		InputManager.ChangeTurns();
	}

	public void TrainUnits()
	{
		List<Building> productionBuildings = GridManager.GetFriendlyProductionBuidlings();
		for (int i = 0; i < productionBuildings.Count; i++)
		{
			Building building = productionBuildings[i];
			if (GridManager.GetUnits().ContainsKey(building.transform.position)) productionBuildings.Remove(building);
		}
		int funds = BattleManager.GetFundsForCurrentPlayer();
		if (UnityEngine.Random.Range(0,3) == 0) funds -= 100;
		List<GameObject> unitPrefabsToTrain = ChooseUnitsToTrain(funds, productionBuildings.Count);
		for (int i = 0; i < unitPrefabsToTrain.Count; i++)
		{
			int r = UnityEngine.Random.Range(0, productionBuildings.Count);
			Building building = productionBuildings[r];
			productionBuildings.Remove(building);
			GridManager.AddUnit(unitPrefabsToTrain[i], building.transform.position);
			BattleManager.ChangeFunds(-1 * unitPrefabsToTrain[i].GetComponent<Unit>().cost);
		}
	}

	public List<GameObject> ChooseUnitsToTrain(int funds, int numProduction)
	{
		List<List<GameObject>> potentialPrefabLists = new List<List<GameObject>>();
		if (numProduction == 0) return new List<GameObject>();
		float[] productionWeights = GetProductionWeights();
		float fundsPerProduction = Math.Min(funds / numProduction, 1000);
		float highestPower = 0f;
		foreach (GameObject unitPrefab in GridManager.GetUnitPrefabs())
		{
			Unit unit = unitPrefab.GetComponent<Unit>();
			if (unit.cost <= funds && unit.cost >= fundsPerProduction && unit.owner == BattleManager.GetCurrentPlayerIndex())
			{
				float power = unit.GetPower() * productionWeights[(int)unit.type];
				List<GameObject> rPrefabs = ChooseUnitsToTrain(funds - unit.cost, numProduction - 1);
				foreach (GameObject rPrefab in rPrefabs)
				{
					Unit rUnit = rPrefab.GetComponent<Unit>();
					power += rUnit.GetPower() * productionWeights[(int)rUnit.type];
				}
				if (power > highestPower)
				{
					highestPower = power;
					potentialPrefabLists.Clear();
					if (!AtCaptureQuota() && (unit.type == UnitType.footman || unit.type == UnitType.archer))
					{
						unit.behaviour = Behaviour.capture;
					}
					rPrefabs.Insert(0, unitPrefab);
					potentialPrefabLists.Add(rPrefabs);
				} else if (power == highestPower) {
					rPrefabs.Insert(0, unitPrefab);
					potentialPrefabLists.Add(rPrefabs);
				}
			}
		}
		// Debug.Log("Potential units to train:");
		// LogPotentialUnitsToTrain(potentialPrefabLists);
		if (potentialPrefabLists.Count == 0) return new List<GameObject>();
		int r = UnityEngine.Random.Range(0,potentialPrefabLists.Count);
		return potentialPrefabLists[r];
	}

	public IEnumerator CheckMoves()
	{
		InfluenceTwo.SetUpMaps();
		// Debug.Log("Influence:");
		// Influence.LogMap(Influence.influenceMap, false);
		// Debug.Log("One Turn Influence:");
		// Influence.LogMap(Influence.oneTurnInfluenceMap, false);
		// Debug.Log("Tension:");
		// Influence.LogMap(Influence.tensionMap, true);
		// Debug.Log("Tension points:");
		// Influence.LogTensionPoints();
		Debug.Log("enemyOneTurnInfluenceMap:");
		InfluenceTwo.LogMap(InfluenceTwo.enemyOneTurnInfluenceMap);
		// Debug.Log("enemyOneTurnInfluence points:");
		// InfluenceTwo.LogPoints(InfluenceTwo.maxEnemyOneTurnInfluencePoints);
		for (int i = 0; i < units.Count; i++)
		{
			Unit unit = units[i];
			if (unit.Equals(null)) 
			{
				break;
			}
			Vector2 moveGoal = ChooseMoveGoal(unit);
			int[,,] distanceMap = Pather.GetDistanceMap(unit, moveGoal);
			int[] bestDistance = new int[2] { 1000, 1000 };
			List<Vector2> candidateMoves = new List<Vector2>();
			foreach (Vector2 movePosition in Pather.GetAIMovePositions(unit, false))
			{
				if (movePosition != GridManager.GetCastleLocationForOwner(BattleManager.GetNextPlayerIndex()) ||
					unit.behaviour == Behaviour.capture)
				{
				// if (InfluenceTwo.oneTurnInfluenceMap[(int)movePosition.x,(int)movePosition.y] <= safeInfluence)
				// {
					if (distanceMap[(int)movePosition.x,(int)movePosition.y,0] < bestDistance[0])
					{
						bestDistance[0] = distanceMap[(int)movePosition.x,(int)movePosition.y,0];
						bestDistance[1] = distanceMap[(int)movePosition.x,(int)movePosition.y,1];
						candidateMoves.Clear();
						candidateMoves.Add(movePosition);
					} else if (distanceMap[(int)movePosition.x,(int)movePosition.y,0] == bestDistance[0]) {
						if (distanceMap[(int)movePosition.x,(int)movePosition.y,1] < bestDistance[1])
						{
							bestDistance[0] = distanceMap[(int)movePosition.x,(int)movePosition.y,0];
							bestDistance[1] = distanceMap[(int)movePosition.x,(int)movePosition.y,1];
							candidateMoves.Clear();
							candidateMoves.Add(movePosition);
						} else if (distanceMap[(int)movePosition.x,(int)movePosition.y,1] == bestDistance[1]) {
							candidateMoves.Add(movePosition);
						}
					}
				// }
				}
			}
			if (candidateMoves.Count == 0) candidateMoves.Add(unit.transform.position);
			int r = UnityEngine.Random.Range(0,candidateMoves.Count);
			Vector2 finalMove = candidateMoves[r];
			Debug.LogFormat("{0} {1} moving from ({2},{3}) to ({4},{5}) en route to ({6},{7})", unit.type.ToString(), unit.owner.ToString(), 
				unit.transform.position.x.ToString(), unit.transform.position.y.ToString(), 
				finalMove.x.ToString(), finalMove.y.ToString(), moveGoal.x.ToString(), moveGoal.y.ToString());
			// Pather.LogDistanceMap(distanceMap);
			moving = true;
			GridManager.MoveUnitAlongPath(unit, finalMove, Pather.GetPathToPoint(finalMove), () => { 
				moving = false;
			});
			while (moving) {
				yield return new WaitForSeconds(.1f);
			}
			if ((Vector2)unit.transform.position == moveGoal && GridManager.CanUnitCapture(unit))
			{
				CaptureBuilding(unit);
			}
			unit.Deactivate();
		}
		inSubCoroutine = false;
	}

	public Vector2 ChooseMoveGoal(Unit unit)
	{
		Vector2 moveGoal = new Vector2();
		if (unit.behaviour == Behaviour.capture)
		{
			if (unit.captureAssignment != new Vector2(-100,-100)) return unit.captureAssignment;
			int lowestDistance = 1000;
			foreach (Building building in GridManager.GetEnemyBuildings())
			{
				if (!captureAssignments.Contains(building.transform.position))
				{
					int dx = (int)Math.Abs(unit.transform.position.x - building.transform.position.x);
					int dy = (int)Math.Abs(unit.transform.position.y - building.transform.position.y);
					int d = dx + dy;
					if (d < lowestDistance) 
					{
						lowestDistance = d;
						moveGoal = building.transform.position;
					}
				}
			}
			unit.captureAssignment = moveGoal;
			captureAssignments.Add(moveGoal);
			return moveGoal;
		}
		// int r = UnityEngine.Random.Range(0,Influence.maxTensionPoints.Count);
		// moveGoal = Influence.maxTensionPoints[r];
		int r = UnityEngine.Random.Range(0,InfluenceTwo.maxEnemyOneTurnInfluencePoints.Count);
		moveGoal = InfluenceTwo.maxEnemyOneTurnInfluencePoints[r];
		return moveGoal;
	}

	public void CaptureBuilding(Unit unit)
	{
		Building building = (Building)GridManager.GetTile(unit.transform.position);
		building.Capture(unit);
		if (building.owner == BattleManager.GetCurrentPlayerIndex())
		{
			CancelCaptureAssignment(unit.captureAssignment);
			unit.captureAssignment = new Vector2(-100,-100);
		}
	}

	public void UpdateUnitListOnDestroy(Unit unit)
	{
		if (units.Contains(unit))
		{
			units.Remove(unit);
		}
		CancelCaptureAssignment(unit.captureAssignment);
	}

	public void CancelCaptureAssignment(Vector2 assignment)
	{
		if (captureAssignments.Contains(assignment))
		{
			captureAssignments.Remove(assignment);
		}
	}

	public IEnumerator CheckAttacks()
	{
		attackFound = false;
		for (int i = 0; i < units.Count; i++)
		{
			Unit unit = units[i];
			if (unit.Equals(null)) 
			{
				break;
			}
			// try capture
			if (GridManager.CanUnitCapture(unit))
			{
				Building building = (Building)GridManager.GetTile(unit.transform.position);
				if (building.controlPoints <= unit.HealthInt())
				{
					CaptureBuilding(unit);
					unitsToRemove.Add(unit);
					unit.Deactivate();
					break;
				}
			}
			Vector2 attackPosition = new Vector2();
			Unit target = ChooseTarget(unit, ref attackPosition);
			if (target)
			{
				moving = true;
				GridManager.MoveUnitAlongPath(unit, attackPosition, Pather.GetPathToPoint(attackPosition), () => { 
					moving = false;
				});
				while (moving) {
					yield return new WaitForSeconds(.1f);
				}
				CompleteAttack(unit, target);
				attackFound = true;
			}
		}
		RemoveUsedUnits();
		inSubCoroutine = false;
	}

	public void CompleteAttack(Unit unit, Unit target)
	{
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
		foreach (Vector2 position in Pather.GetAIMovePositions(unit, true))
		{
			GridManager.MoveUnit(startingPosition, position, () => {});
			foreach (Vector2 targetPosition in GridManager.GetCoordsToAttackHighlight(unit.transform.position, unit.range))
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

	// production weight methods
	public float[] GetProductionWeights()
	{
		float[] productionWeights = new float[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
		productionWeights[0] = GetFootmanWeight();
		return productionWeights;
	}

	public float GetFootmanWeight()
	{
		if (AtCaptureQuota())
		{
			return 1f;
		} else {
			return 1.5f;
		}
	}

	public bool AtCaptureQuota()
	{
		int capturers = 0;
		foreach (Unit unit in GridManager.GetFriendlyUnits().Values)
		{
			if (unit.behaviour == Behaviour.capture) capturers++;
		}
		if (capturers >= GridManager.GetEnemyBuildings().Count / 2)
		{
			return true;
		} else {
			return false;
		}
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

	public void LogPotentialUnitsToTrain(List<List<GameObject>> potentialLists)
	{
		foreach (List<GameObject> list in potentialLists)
		{
			string str = "( ";
			foreach (GameObject unitPrefab in list)
			{
				Unit unit = unitPrefab.GetComponent<Unit>();
				str = str + unit.type.ToString() + ", ";
			}
			str = str + ")";
			Debug.Log(str);
		}
	}

}
