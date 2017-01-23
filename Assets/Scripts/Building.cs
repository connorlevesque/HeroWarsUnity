using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Building : Tile {

	public int owner;
	public int controlPoints;
	public int revenue;

	public Sprite[] ownerSprites;
	private SpriteRenderer spriteRenderer;
	private GameObject canvas;
	private Text controlText;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		canvas = transform.GetChild(0).gameObject;
		controlText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
	}

	void OnMouseDown() 
	{
		InputManager.BuildingClicked(this);
	}

	public void Capture(Unit unit)
	{
		controlPoints -= unit.HealthInt();
		controlText.text = controlPoints.ToString();
		if (controlPoints <= 0)
		{
			controlPoints = 20;
			owner = unit.owner;
			spriteRenderer.sprite = ownerSprites[owner];
			canvas.SetActive(false);
		} else if (controlPoints < 20) {
			canvas.SetActive(true);
		} else {
			canvas.SetActive(false);
		}
		GridManager.UpdateBuilding(this);
	}

	public void RefreshControlPoints()
	{
		controlPoints = 20;
		canvas.SetActive(false);
		GridManager.UpdateBuilding(this);
	}
}
