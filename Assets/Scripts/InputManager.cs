using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	private static InputManager instance;

	private UIManager uiManager;
	private ComputerOpponent computerOpponent;
	private Stack<InputState> states = new Stack<InputState>();
	private Unit selectedUnit;
	private GameObject selectedPrefab;
	private Building selectedBuilding;
	public bool canReceiveInput = true;
	private string menuType;

	void Awake() {
		instance = this;
	}

	void Start() {
		states.Push(new BaseState());
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
		computerOpponent = GetComponent<ComputerOpponent>();
	}

	public static void HandleInput(string input, params object[] context) {
		if (CanReceiveInput()) CurrentState().HandleInput(input, context);
	}

	public static void TransitionTo(InputState newState) {
		CurrentState().Exit();
		if (newState.Name() == "BaseState") instance.states.Clear();
		instance.states.Push(newState);
		newState.Enter();
	}

	public static void TransitionBack() {
		if (CurrentState().Name() == "BaseState") return;
		CurrentState().Exit();
		instance.states.Pop();
		CurrentState().Enter();
	}

	public static InputState CurrentState() {
		return instance.states.Peek();
	}

	public static bool CanReceiveInput() {
		return instance.canReceiveInput;
	}

	public static void SetReceiveInput(bool result) {
		instance.canReceiveInput = result;
	}

	public static bool IsPointerOverUIButton() {
		if (EventSystem.current.currentSelectedGameObject) {
			GameObject pointerObject = EventSystem.current.currentSelectedGameObject;
			if (pointerObject.transform.parent) {
				GameObject parent = pointerObject.transform.parent.gameObject;
				if (parent.CompareTag("UIManager")) {
					return true;
				}
			}
		}
		return false;
	}

	// turn controls
	public static void ChangeTurns() {
		EndTurn();
		TransitionTo(new BaseState());
		StartTurn();
	}

	public static void EndTurn() {
		GridManager.ActivateUnits();
		GridManager.RefreshControlPoints();
		BattleManager.EndTurn();
	}

	public static void StartTurn() {
		BattleManager.StartTurn();
		instance.uiManager.UpdateFundsDisplay();
		GridManager.HealUnits();
		bool isComputerTurn = BattleManager.GetCurrentPlayerType() == PlayerType.computer;
		if (isComputerTurn) {
			SetReceiveInput(false);
			instance.StartCoroutine(instance.computerOpponent.Run());
		} else {
			SetReceiveInput(true);
		}
	}

	public static void WinLoseLevel(string winOrLose) {
		instance.computerOpponent.StopAllCoroutines();
		TransitionTo(new GameOverState(winOrLose));
	}

	public static void UpdateAIUnitListOnDestroy(Unit unit) {
		instance.computerOpponent.UpdateUnitListOnDestroy(unit);
	}
}
