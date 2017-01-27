using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum UnitType { footman, archer, scout, catapult, knight, guard, bombard, greatKnight };
public enum UnitGroup { infantry, cavalry, artillery, flying };
public enum Behaviour { none, hold, defend };

public class Unit : MonoBehaviour {

	// general properties
	public UnitType type;
	public UnitGroup grouping;
	public bool activated = true;
	public int owner;
	public int movePoints;
	public int[] range = new int[2];
	// ride/drop properties
	public bool canCarry = false;
	public Unit cargo;
	// combat properties
	public int cost;
	public int health = 100;
	public int damage;
	public int bonusDamage;
	public float pen;
	public float bonusPen;
	public UnitGroup bonusCondition;
	public float armor;
	// UI references
	private Color inactiveColor = Color.gray;
	private SpriteRenderer spriteRenderer;
	private GameObject healthLabel;
	private Text healthText;
	private GameObject carryLabel;
	private Text carryText;
	private GameObject damageLabel;
	private Text damageText;
	// AI properties
	public float powerConstant = 1;
	public Behaviour behaviour = Behaviour.none;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		healthLabel = transform.GetChild(0).GetChild(0).gameObject;
		healthText = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
		carryLabel = transform.GetChild(0).GetChild(1).gameObject;
		carryText = transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
		damageLabel = transform.GetChild(0).GetChild(2).gameObject;
		damageText = transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>();
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
			healthLabel.SetActive(true);
		} else {
			healthText.text = "";
			healthLabel.SetActive(false);
		}
		GridManager.UpdateUnit(this);
	}

	public int HealthInt()
	{
		return Mathf.CeilToInt(health / 10f);
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

	public void ShowDamageLabel(int amount)
	{
		damageText.text = amount.ToString();
		damageLabel.SetActive(true);
	}

	public void HideDamageLabel()
	{
		damageLabel.SetActive(false);
	}

	// AI relevant methods
	public float GetPower()
	{
		return cost * powerConstant * health / 100f;
	}

}
