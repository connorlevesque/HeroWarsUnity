using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProductionSlot : MonoBehaviour {

	public GameObject unitPrefab;

	private Image image;
	private Text typeText;
	private Image checkMark;
	private Text costText;
	private Button button;

	public void SlotClicked()
	{
		transform.parent.GetComponent<ProductionMenu>().ClearCheckMarks();
		checkMark.gameObject.SetActive(true);
		InputManager.HandleInput("tapProductionSlot", unitPrefab);
	}

	public void SetSlotForUnit(GameObject uP)
	{
		unitPrefab = uP;
		image = transform.GetChild(0).GetComponent<Image>();
		typeText = transform.GetChild(1).GetComponent<Text>();
		checkMark = transform.GetChild(2).GetComponent<Image>();
		costText = transform.GetChild(3).GetComponent<Text>();
		button = transform.GetChild(4).GetComponent<Button>();
		button.onClick.AddListener(() => SlotClicked());
		image.sprite = unitPrefab.GetComponent<SpriteRenderer>().sprite;
		typeText.text = Capitalize(unitPrefab.GetComponent<Unit>().type.ToString());
		int cost = unitPrefab.GetComponent<Unit>().cost;
		costText.text = cost.ToString();
		if (cost > BattleManager.GetFundsForCurrentPlayer())
		{
			button.interactable = false;
			GetComponent<Image>().color = Color.gray;
		}
	}

	private string Capitalize(string s)
	{
		return char.ToUpper(s[0]) + s.Substring(1);
	}

	public void SetCheckMark(bool onOff)
	{
		checkMark.gameObject.SetActive(onOff);
	}

}
