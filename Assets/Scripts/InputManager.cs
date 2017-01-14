using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	private static InputManager instance;

	private GridManager gridManager;
	private UIManager uiManager;

	private string inputState;
	private Unit selectedUnit;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		gridManager = GameObject.FindWithTag("GridManager").GetComponent<GridManager>();
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
	}

	public static void UnitClicked(Unit unit)
	{
		instance.selectedUnit = unit;
		List<Vector2> coords = Pather.GetCoordsToMoveHighlight(unit);
		instance.uiManager.HighlightMoveTiles(coords);
	}

	public static void HighlightClicked(string color)
	{
		Debug.LogFormat("Highlight {0} clicked", color);
	}

}
