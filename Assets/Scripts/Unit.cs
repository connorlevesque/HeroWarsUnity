using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public enum UnitType { footman, archer, barbarian, guard, scout, knight, greatKnight, catapult, bombard };
	public enum UnitGroup { infantry, cavalry, artillery, flying }

	public UnitType type;
	public UnitGroup grouping;
	public int owner;
	public int movement;
	public int[] range = new int[2];
	public int cost;
	public int health = 100;
	// ride/drop action variables
	// public bool canCarry = false;
	// public Unit cargo;

	// combat properties

	void OnMouseUpAsButton()
	{
		InputManager.UnitClicked(this);
	}

}
