using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum UnitType { footman, archer, barbarian, guard, scout, knight, greatKnight, catapult, bombard };
public enum UnitGroup { infantry, cavalry, artillery, flying }

public class Unit : MonoBehaviour {

	public UnitType type;
	public UnitGroup grouping;
	public bool activated = true;
	public int owner;
	public int movePoints;
	public int[] range = new int[2];
	public int cost;
	public int health = 100;
	// ride/drop action variables
	// public bool canCarry = false;
	// public Unit cargo;

	// combat properties

	public bool active = true;
	private Color inactiveColor = Color.gray;
	private SpriteRenderer spriteRenderer;
	private GameObject canvas;
	private Text healthText;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		canvas = transform.GetChild(0).gameObject;
		healthText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
	}

	void OnMouseUpAsButton()
	{
		InputManager.UnitClicked(this);
	}

	public void ChangeHealth(int amount)
	{
		health += amount;
		if (health > 100) health = 100;
		int h = HealthInt();
		if (h <= 9)
		{
			healthText.text = h.ToString();
			canvas.SetActive(true);
		} else {
			healthText.text = "";
			canvas.SetActive(false);
		}
		GridManager.UpdateUnit(this);
	}

	public int HealthInt()
	{
		return Mathf.CeilToInt(health / 10);
	}

	public void Activate()
	{
		activated = true;
		spriteRenderer.color = Color.white;
	}

	public void Deactivate()
	{
		activated = false;
		spriteRenderer.color = inactiveColor;
	}

}
