using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	// Buttons
	public GameObject gameMenuBtn;
	public GameObject nextBtn;
	public GameObject endTurnBtn;
   public GameObject gameMenu;
	public GameObject fundsDisplay;
	public ProductionMenu productionMenu;
	public GameObject okayMenu;
	public GameObject okayBtn;
	// Highlights
	private List<GameObject> highlights = new List<GameObject>();
   private List<GameObject> highlightPath = new List<GameObject>();
	public GameObject highlightBlue;
	public GameObject highlightRed;

   public GameObject confirmMenuPrefab;

	void Start() {
		SetUpButtons();
		UpdateFundsDisplay();
	}

	private void SetUpButtons() {
		SetUpButton(gameMenuBtn, "gameMenuBtn");
		SetUpButton(endTurnBtn, "endTurnBtn");
		SetUpButton(nextBtn, "nextBtn");
		SetUpButton(okayBtn, "okayBtn");
	}

	private void SetUpButton(GameObject buttonObject, string input) {
		Button button = buttonObject.GetComponent<Button>();
		button.onClick.AddListener(() => InputManager.HandleInput(input));
	}

	public void Highlight(List<Vector2> coords, string color, bool path=false) {
		GameObject highlight = highlightBlue;
		if (color == "blue") highlight = highlightBlue;
		if (color == "red") highlight = highlightRed;

		foreach (Vector2 coord in coords) {
			Vector3 position = new Vector3(coord.x, coord.y, -1);
			highlight = (GameObject) Instantiate(highlight, position, Quaternion.identity);
			if (path) {
            highlightPath.Add(highlight);
         } else {
            highlights.Add(highlight);
         }
		}
	}

   public bool IsPointHighlighted(Vector2 point) {
      foreach (GameObject highlightGob in highlights) {
         Highlight highlight = highlightGob.GetComponent<Highlight>();
         bool pointIsHighlighted = (Vector2) highlight.transform.position == point;
         if (pointIsHighlighted) return true;
      }
      return false;
   }

	public void RemoveHighlights() {
		foreach (GameObject highlight in highlights) {
			Destroy(highlight);
		}
		highlights.Clear();
	}
	
   public void RemoveHighlightPath() {
      foreach (GameObject highlight in highlightPath) {
         Destroy(highlight);
      }
      highlightPath.Clear();
   }

	public void UpdateFundsDisplay() {
		fundsDisplay.transform.GetChild(0).GetComponent<Text>().text = BattleManager.GetFundsForCurrentPlayer().ToString();
		if (BattleManager.GetCurrentPlayerIndex() == 1) {
			fundsDisplay.transform.GetChild(0).GetComponent<Text>().color = Color.red;
		} else if (BattleManager.GetCurrentPlayerIndex() == 2) {
			fundsDisplay.transform.GetChild(0).GetComponent<Text>().color = Color.blue;
		}
	}

   public void OpenConfirmMenu(string message, Action onYes, Action onNo) {
      GameObject confirmMenuGob = Instantiate(confirmMenuPrefab, transform);
      ConfirmMenu confirmMenu = confirmMenuGob.GetComponent<ConfirmMenu>();
      confirmMenu.message.text = message;
      confirmMenu.onYes = onYes;
      confirmMenu.onNo = onNo;
   }

	// Show/Hide UI for state methods
	public void ShowTargetUI(List<Vector2> highlightCoords) {
		Highlight(highlightCoords, "red");
	}

	public void HideTargetUI() {
		RemoveHighlights();
	}

	public void ShowRangeUI(List<Vector2> highlightCoords) {
		Highlight(highlightCoords, "red");
	}

	public void HideRangeUI() {
		RemoveHighlights();
	}

	public void ShowProductionUI(GameObject[] unitPrefabs) {
		productionMenu.CreateProductionSlots(unitPrefabs);
		productionMenu.gameObject.SetActive(true);
	}

	public void HideProductionUI() {
		productionMenu.gameObject.SetActive(false);
	}

	public void ShowWinLoseUI(string message) {
		okayMenu.transform.GetChild(1).GetComponent<Text>().text = message;
		okayMenu.SetActive(true);
	}

	public void HideWinLoseUI() {
		okayMenu.SetActive(false);
	}
}
