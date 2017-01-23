using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	private static InputManager instance;

	private UIManager uiManager;
	private Stack<string> states = new Stack<string>();
	private Unit selectedUnit;
	private GameObject selectedPrefab;
	private Building selectedBuilding;
	public bool canReceiveInput = true;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
	}

	public static bool CanReceiveInput()
	{
		return instance.canReceiveInput;
	}

	public static void SetReceiveInput(bool result)
	{
		instance.canReceiveInput = result;
	}

	// from base state
	public static void GameMenuBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "base")
		{
			ExitState("base");
			instance.states.Push("gameMenu");
			EnterState("gameMenu");
		}
	}

	public static void UnitClicked(Unit unit)
	{
		if (CanReceiveInput() && CurrentState() == "base")
		{
			if (unit.owner == BattleManager.GetCurrentPlayerIndex() && unit.activated)
			{
				instance.selectedUnit = unit;
				ExitState("base");
				instance.states.Push("action");
				EnterState("action");
			} else {
				// go to range state
			}
		}
	}

	public static void BuildingClicked(Building building)
	{
		if (CanReceiveInput() && CurrentState() == "base")
		{
			if (building.owner == BattleManager.GetCurrentPlayerIndex())
			{
				instance.selectedBuilding = building;
				ExitState("base");
				instance.states.Push("production");
				EnterState("production");
			} else {
				// go to range state
			}
		}
	}

	// from multiple states
	public static void BackBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() != "base")
		{
			if (CurrentState() == "action")
			{
				Vector2 position = instance.selectedUnit.transform.position;
				if (position != Pather.center)
				{
					GridManager.MoveUnit(position, Pather.center);
					EnterState("action");
				} else {
					ExitState(instance.states.Pop());
					bool canCapture = GridManager.CanUnitCapture(instance.selectedUnit);
					instance.uiManager.ShowActionUI(new List<Vector2>(), canCapture);
				}
			} else {
				ExitState(instance.states.Pop());
				EnterState(CurrentState());
			}
		}
	}

	public static void NextBtnClicked()
	{
		Debug.Log("Next clicked");
	}

	// from action state
	public static void MoveHighlightClicked(Vector2 position)
	{
		if (CanReceiveInput() && CurrentState() == "action")
		{
			List<Vector2> path = Pather.GetPathToPoint(position);
			SetReceiveInput(false);
			GridManager.MoveUnitAlongPath(instance.selectedUnit, position, path);
			instance.uiManager.RemoveHighlights();
		}
	}

	public static void CheckCanCapture()
	{
		if (CurrentState() == "action")
		{
			instance.uiManager.ToggleCaptureBtn(GridManager.CanUnitCapture(instance.selectedUnit));
		}
	}

	public static void AttackBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "action")
		{
			ExitState("action");
			instance.states.Push("target");
			EnterState("target");
		}
	}

	public static void CaptureBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "action")
		{
			Tile tile = GridManager.GetTile(instance.selectedUnit.transform.position);
			if (tile.isBuilding)
			{
				Building building = (Building)tile;
				building.Capture(instance.selectedUnit);
				instance.selectedUnit.Deactivate();
				ExitState("action");
				instance.states.Clear();
				EnterState("base");	
			}
		}
	}

	public static void WaitBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "action")
		{
			instance.selectedUnit.Deactivate();
			ExitState("action");
			instance.states.Clear();
			EnterState("base");
		}
	}

	// from target state
	public static void AttackHighlightClicked(Vector2 position)
	{
		if (CanReceiveInput() && CurrentState() == "target")
		{
			GridManager.CalculateAttack(instance.selectedUnit, GridManager.GetUnit(position));
			instance.selectedUnit.Deactivate();
			ExitState("target");
			instance.states.Clear();
			EnterState("base");
		}
	}

	// from production state
	public static void ProductionSlotClicked(GameObject unitPrefab)
	{
		if (CanReceiveInput() && CurrentState() == "production")
		{
			instance.selectedPrefab = unitPrefab;
		}
	}

	public static void TrainBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "production" && instance.selectedPrefab != null)
		{
			BattleManager.ChangeFunds(instance.selectedPrefab.GetComponent<Unit>().cost * -1);
			instance.uiManager.UpdateFundsDisplay();
			GridManager.AddUnit(instance.selectedPrefab, instance.selectedBuilding.transform.position);
			ExitState("production");
			instance.states.Clear();
			EnterState("base");
		}
	}

	// from gameMenu state
	public static void EndTurnBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "gameMenu")
		{
			ExitState("gameMenu");
			GridManager.ActivateUnits();
			GridManager.RefreshControlPoints();
			BattleManager.EndTurn();
			instance.states.Clear();
			EnterState("base");
			BattleManager.StartTurn();
			instance.uiManager.UpdateFundsDisplay();
		}
	}

	// Enter/Exit state methods
	public static void EnterState(string newState)
	{
		if (newState == "base")
		{
			instance.uiManager.ShowBaseUI();
		} else if (newState == "action") {
			List<Vector2> coords = Pather.GetCoordsToMoveHighlight(instance.selectedUnit);
			bool canCapture = GridManager.CanUnitCapture(instance.selectedUnit);
			instance.uiManager.ShowActionUI(coords, canCapture);
		} else if (newState == "target") {
			List<Vector2> coords = GridManager.GetCoordsToAttackHighlight(instance.selectedUnit);
			instance.uiManager.ShowTargetUI(coords);
		} else if (newState == "confirm") {
			instance.uiManager.ShowConfirmUI();
		} else if (newState == "range") {
			instance.uiManager.ShowRangeUI();
		} else if (newState == "production") {
			instance.uiManager.ShowProductionUI(GridManager.GetUnitPrefabs());
		} else if (newState == "gameMenu") {
			instance.uiManager.ShowGameMenuUI();
		}
	}

	public static void ExitState(string oldState)
	{
		if (oldState == "base")
		{
			instance.uiManager.HideBaseUI();
		} else if (oldState == "action") {
			instance.uiManager.HideActionUI();
		} else if (oldState == "target") {
			instance.uiManager.HideTargetUI();
		} else if (oldState == "confirm") {
			instance.uiManager.HideConfirmUI();
		} else if (oldState == "range") {
			instance.uiManager.HideRangeUI();
		} else if (oldState == "production") {
			instance.uiManager.HideProductionUI();
		} else if (oldState == "gameMenu") {
			instance.uiManager.HideGameMenuUI();
		}
	}

	// state accessor
	public static string CurrentState()
	{
		if (instance.states.Count == 0)
		{
			return "base";
		} else {
			return instance.states.Peek();
		}
	}

}
