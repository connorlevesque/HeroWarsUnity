using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	private static InputManager instance;

	private UIManager uiManager;
	private ComputerOpponent computerOpponent;
	private Stack<string> states = new Stack<string>();
	private Unit selectedUnit;
	private GameObject selectedPrefab;
	private Building selectedBuilding;
	public bool canReceiveInput = true;
	private string menuType;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
		computerOpponent = GetComponent<ComputerOpponent>();
	}

	public static bool CanReceiveInput()
	{
		return instance.canReceiveInput;
	}

	public static void SetReceiveInput(bool result)
	{
		instance.canReceiveInput = result;
	}

	public static bool IsPointerOverUIButton()
	{
		if (EventSystem.current.currentSelectedGameObject)
		{
			GameObject pointerObject = EventSystem.current.currentSelectedGameObject;
			if (pointerObject.transform.parent)
			{
				GameObject parent = pointerObject.transform.parent.gameObject;
				if (parent.CompareTag("UIManager"))
				{
					return true;
				}
			}
		}
		return false;
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
				instance.selectedUnit = unit;
				ExitState("base");
				instance.states.Push("range");
				EnterState("range");
			}
		}
	}

	public static void BuildingClicked(Building building)
	{
		if (CanReceiveInput() && CurrentState() == "base")
		{
			if (building.owner == BattleManager.GetCurrentPlayerIndex() &&
				building.type == TileType.barracks)
			{
				instance.selectedBuilding = building;
				ExitState("base");
				instance.states.Push("production");
				EnterState("production");
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
					GridManager.MoveUnit(position, Pather.center, 
						() => { InputManager.CheckActions();
								InputManager.SetReceiveInput(true);});
					EnterState("action");
				} else {
					ExitState(instance.states.Pop());
					EnterState(CurrentState());
				}
			} else {
				ExitState(instance.states.Pop());
				if (CurrentState() == "action")
				{
					Vector2 position = instance.selectedUnit.transform.position;
					if (position == Pather.center)
					{
						EnterState("action");
					} else {
						bool canCapture = GridManager.CanUnitCapture(instance.selectedUnit);
						instance.uiManager.ShowActionUI(new List<Vector2>(), canCapture, CanUseAttackAction(), false, false);
					}
				} else {
					EnterState(CurrentState());
				}
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
			GridManager.MoveUnitAlongPath(instance.selectedUnit, position, path, 
				() => { InputManager.CheckActions();
						InputManager.SetReceiveInput(true);});
			instance.uiManager.RemoveHighlights();
		}
	}

	public static void CheckActions()
	{
		if (CurrentState() == "action")
		{
			instance.uiManager.ToggleCaptureBtn(GridManager.CanUnitCapture(instance.selectedUnit));
			instance.uiManager.ToggleAttackBtn(CanUseAttackAction());
			instance.uiManager.ToggleRideBtn(false);
			instance.uiManager.ToggleDropBtn(false);
		}
	}

	public static bool CanUseAttackAction()
	{
		if (instance.selectedUnit.grouping == UnitGroup.artillery && HasMoved())
		{
			return false;
		} else {
			return true;
		}
	}

	public static bool HasMoved()
	{
		Vector2 position = instance.selectedUnit.transform.position;
		if (position == Pather.center)
		{
			return false;
		} else {
			return true;
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

	public static void RideBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "action")
		{
			Debug.Log("Ride Button Clikced");
			// Tile tile = GridManager.GetTile(instance.selectedUnit.transform.position);
			// if (tile.isBuilding)
			// {
			// 	Building building = (Building)tile;
			// 	building.Capture(instance.selectedUnit);
			// 	instance.selectedUnit.Deactivate();
			// 	ExitState("action");
			// 	instance.states.Clear();
			// 	EnterState("base");	
			// }
		}
	}

	public static void DropBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "action")
		{
			Debug.Log("Drop Button Clikced");
			// Tile tile = GridManager.GetTile(instance.selectedUnit.transform.position);
			// if (tile.isBuilding)
			// {
			// 	Building building = (Building)tile;
			// 	building.Capture(instance.selectedUnit);
			// 	instance.selectedUnit.Deactivate();
			// 	ExitState("action");
			// 	instance.states.Clear();
			// 	EnterState("base");	
			// }
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
			Unit target = GridManager.GetUnit(position);
			if (target)
			{
				if (target.owner != BattleManager.GetCurrentPlayerIndex())
				{
					GridManager.CalculateAttack(instance.selectedUnit, target);
					instance.selectedUnit.Deactivate();
					ExitState("target");
					instance.states.Clear();
					EnterState("base");
				}
			}
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
			EndTurn();
			EnterState("base");
			StartTurn();
		}
	}

	public static void SaveBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "gameMenu")
		{
			// // save game here
			// ExitState("gameMenu");
			// EnterState("base");
		}
	}

	public static void RestartBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "gameMenu")
		{
			instance.menuType = "restart";
			ExitState("gameMenu");
			instance.states.Push("confirmChangeScene");
			EnterState("confirmChangeScene");
		}
	}

	public static void QuitBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "gameMenu")
		{
			instance.menuType = "quit";
			ExitState("gameMenu");
			instance.states.Push("confirmChangeScene");
			EnterState("confirmChangeScene");
		}
	}

	public static void ChangeTurns()
	{
		ExitState("gameMenu");
		EndTurn();
		EnterState("base");
		StartTurn();
	}

	public static void EndTurn()
	{
		GridManager.ActivateUnits();
		GridManager.RefreshControlPoints();
		BattleManager.EndTurn();
		instance.states.Clear();
	}

	// from confirmChangeScene
	public static void YesBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "confirmChangeScene")
		{
			if (instance.menuType == "restart")
			{
				GameManager.RestartLevel();
			} else if (instance.menuType == "quit") {
				GameManager.LoadSpecificLevel(0);
			}
		}
	}

	public static void NoBtnClicked()
	{
		if (CanReceiveInput() && CurrentState() == "confirmChangeScene")
		{
			ExitState("confirmChangeScene");
			instance.states.Clear();
			EnterState("base");
		}
	}

	// from winLose
	public static void OkayBtnClicked()
	{
		Debug.Log("1");
		Debug.Log("2");
		if (instance.menuType == "win")
		{
			GameManager.LoadNextLevel();
		} else if (instance.menuType == "lose") {
			Debug.Log("3");
			GameManager.RestartLevel();
		}
	}

	// to winLose
	public static void WinLoseLevel(string winOrLose)
	{
		Debug.LogFormat("you {0}", winOrLose);
		instance.computerOpponent.StopAllCoroutines();
		instance.menuType = winOrLose;
		ExitState(CurrentState());
		instance.states.Push("winLose");
		EnterState("winLose");
	}

	// computer opponent relevant
	public static void StartTurn()
	{
		BattleManager.StartTurn();
		instance.uiManager.UpdateFundsDisplay();
		GridManager.HealUnits();
		if (BattleManager.GetCurrentPlayerType() == PlayerType.computer)
		{
			SetReceiveInput(false);
			instance.StartCoroutine(instance.computerOpponent.Run());
		} else if (BattleManager.GetCurrentPlayerType() == PlayerType.local) {
			SetReceiveInput(true);
		}
	}

	public static void CancelCaptureAssignment(Vector2 assignment)
	{
		instance.computerOpponent.CancelCaptureAssignment(assignment);
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
			instance.uiManager.ShowActionUI(coords, canCapture, CanUseAttackAction(), false, false);
		} else if (newState == "target") {
			List<Vector2> coords = GridManager.GetCoordsToAttackHighlight(instance.selectedUnit.transform.position, 
																		  instance.selectedUnit.range);
			GridManager.ShowDamageLabels(coords, instance.selectedUnit);
			instance.uiManager.ShowTargetUI(coords);
		} else if (newState == "confirm") {
			instance.uiManager.ShowConfirmUI();
		} else if (newState == "range") {
			instance.uiManager.ShowRangeUI(GetRangeStateCoords(instance.selectedUnit));
		} else if (newState == "production") {
			instance.uiManager.ShowProductionUI(GridManager.GetUnitPrefabs());
		} else if (newState == "gameMenu") {
			instance.uiManager.ShowGameMenuUI();
		} else if (newState == "confirmChangeScene") {
			string message = "";
			if (instance.menuType == "restart")
			{
				message = "Are you sure you want to restart the level?";
			} else if (instance.menuType == "quit") {
				message = "Are you sure you want to quit to the start screen?";
			}
			instance.uiManager.ShowChangeSceneUI(message);
		} else if (newState == "winLose") {
			string message = "";
			if (instance.menuType == "win")
			{
				message = "You won!";
			} else if (instance.menuType == "lose") {
				message = "You lost!";
			}
			instance.uiManager.ShowWinLoseUI(message);
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
			GridManager.HideDamageLabels();
			instance.uiManager.HideTargetUI();
		} else if (oldState == "confirm") {
			instance.uiManager.HideConfirmUI();
		} else if (oldState == "range") {
			instance.uiManager.HideRangeUI();
		} else if (oldState == "production") {
			instance.uiManager.HideProductionUI();
		} else if (oldState == "gameMenu") {
			instance.uiManager.HideGameMenuUI();
		} else if (oldState == "confirmChangeScene") {
			instance.uiManager.HideChangeSceneUI();
		} else if (oldState == "winLose") {
			instance.uiManager.HideWinLoseUI();
		}
	}

	public static List<Vector2> GetRangeStateCoords(Unit unit)
	{
		List<Vector2> coords = new List<Vector2>();
		List<Vector2> movePositions = Pather.GetCoordsToMoveHighlight(unit);
 		foreach (Vector2 movePosition in movePositions)
 		{
			foreach (Vector2 attackPosition in GridManager.GetCoordsToAttackHighlight(movePosition, unit.range))
			{
				if (!coords.Contains(attackPosition)) coords.Add(attackPosition);
			}
		}
		return coords;
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
