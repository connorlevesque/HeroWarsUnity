using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

	void OnMouseUpAsButton() 
	{
		if (!InputManager.IsPointerOverUIButton())
		{
			InputManager.BuildingClicked(this);
		}
	}

	public void Capture(Unit unit)
	{
		int temp = owner;
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
		if (type == TileType.castle)
		{
			CheckIfLastCastle(temp);
		}
	}

	public void RefreshControlPoints()
	{
		controlPoints = 20;
		canvas.SetActive(false);
		GridManager.UpdateBuilding(this);
	}

	public void CheckIfLastCastle(int owner)
	{
		if (GridManager.GetCastleLocationForOwner(owner) == new Vector2(-100,-100))
		{
			if (owner == 1)
			{
				InputManager.WinLoseLevel("lose");
			} else if (owner == 2) {
				InputManager.WinLoseLevel("win");
			}
		}
	}
}
