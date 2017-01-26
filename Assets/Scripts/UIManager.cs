using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	// Buttons
	public GameObject gameMenuBtn;
	public GameObject backBtn;
	public GameObject nextBtn;
	public GameObject attackBtn;
	public GameObject captureBtn;
	public GameObject rideBtn;
	public GameObject dropBtn;
	public GameObject waitBtn;
	public GameObject endTurnBtn;
	public GameObject fundsDisplay;
	public ProductionMenu productionMenu;
	// Highlights
	private List<GameObject> highlights = new List<GameObject>();
	public GameObject highlightBlue;
	public GameObject highlightRed;

	void Start()
	{
		SetUpButtons();
		UpdateFundsDisplay();
	}

	private void SetUpButtons()
	{
		gameMenuBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.GameMenuBtnClicked());
		backBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.BackBtnClicked());
		nextBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.NextBtnClicked());
		attackBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.AttackBtnClicked());
		captureBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.CaptureBtnClicked());
		rideBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.RideBtnClicked());
		dropBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.DropBtnClicked());
		waitBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.WaitBtnClicked());
		endTurnBtn.GetComponent<Button>().onClick.AddListener(() => InputManager.EndTurnBtnClicked());
	}

	// UI helper methods

	public void Highlight(List<Vector2> coords, string color)
	{
		GameObject highlight = highlightBlue;
		if (color == "blue")
		{
			highlight = highlightBlue;
		} else if (color == "red") {
			highlight = highlightRed;
		}
		foreach(Vector2 coord in coords)
		{
			Vector3 position = new Vector3(coord.x, coord.y, -1);
			highlight = (GameObject) Instantiate(highlight, position, Quaternion.identity);
			highlights.Add(highlight);
		}
	}

	public void RemoveHighlights()
	{
		foreach(GameObject highlight in highlights)
		{
			Destroy(highlight);
		}
		highlights.Clear();
	}
	
	public void UpdateFundsDisplay()
	{
		fundsDisplay.transform.GetChild(0).GetComponent<Text>().text = BattleManager.GetFundsForCurrentPlayer().ToString();
		if (BattleManager.GetCurrentPlayerIndex() == 1)
		{
			fundsDisplay.transform.GetChild(0).GetComponent<Text>().color = Color.red;
		} else if (BattleManager.GetCurrentPlayerIndex() == 2) {
			fundsDisplay.transform.GetChild(0).GetComponent<Text>().color = Color.blue;
		}
	}

	// Show/Hide UI for state methods

	// base
	public void ShowBaseUI()
	{
		gameMenuBtn.SetActive(true);
		backBtn.SetActive(false);
	}

	public void HideBaseUI()
	{
		gameMenuBtn.SetActive(false);
		backBtn.SetActive(true);
	}

	// action
	public void ShowActionUI(List<Vector2> highlightCoords, bool canCapture, bool canAttack, bool canRide, bool canDrop)
	{
		attackBtn.SetActive(true);
		ToggleAttackBtn(canAttack);
		ToggleCaptureBtn(canCapture);
		ToggleRideBtn(canRide);
		ToggleDropBtn(canDrop);
		Highlight(highlightCoords, "blue");
	}

	public void HideActionUI()
	{
		attackBtn.SetActive(false);
		captureBtn.SetActive(false);
		waitBtn.SetActive(false);
		RemoveHighlights();
	}

	public void ToggleCaptureBtn(bool turnOn)
	{
		Debug.LogFormat("ToggleCaptureBtn = {0}", turnOn);
		captureBtn.SetActive(turnOn);
		waitBtn.SetActive(!turnOn);
	}

	public void ToggleAttackBtn(bool canAttack)
	{
		attackBtn.GetComponent<Button>().interactable = canAttack;
		if (canAttack)
		{
			attackBtn.GetComponent<Image>().color = Color.white;
		} else {
			attackBtn.GetComponent<Image>().color = Color.gray;
		}
	}

	public void ToggleRideBtn(bool turnOn)
	{
		// rideBtn.SetActive(turnOn);
		// waitBtn.SetActive(!turnOn);
	}

	public void ToggleDropBtn(bool turnOn)
	{
		// dropBtn.SetActive(turnOn);
	}

	// target
	public void ShowTargetUI(List<Vector2> highlightCoords)
	{
		Highlight(highlightCoords, "red");
	}

	public void HideTargetUI()
	{
		RemoveHighlights();
	}

	// confirm
	public void ShowConfirmUI()
	{

	}

	// range
	public void ShowRangeUI()
	{

	}

	public void HideRangeUI()
	{
		
	}

	public void HideConfirmUI()
	{
		
	}

	// Production
	public void ShowProductionUI(GameObject[] unitPrefabs)
	{
		productionMenu.CreateProductionSlots(unitPrefabs);
		productionMenu.gameObject.SetActive(true);
	}

	public void HideProductionUI()
	{
		productionMenu.gameObject.SetActive(false);
	}

	// gameMenu
	public void ShowGameMenuUI()
	{
		endTurnBtn.SetActive(true);
	}

	public void HideGameMenuUI()
	{
		endTurnBtn.SetActive(false);
	}

}
