using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ProductionMenu : MonoBehaviour {

	public GameObject productionSlotPrefab;
	public Vector2 slotPosition;
	public List<GameObject> productionSlots = new List<GameObject>();

	public void CreateProductionSlots(GameObject[] unitPrefabs)
	{
		transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => InputManager.HandleInput("trainBtn"));
		Vector2 position = slotPosition;
		foreach (GameObject unitPrefab in unitPrefabs)
		{
			if (unitPrefab.GetComponent<Unit>().owner == BattleManager.GetCurrentPlayerIndex())
			{
				GameObject productionSlot = (GameObject)Instantiate(productionSlotPrefab, position + new Vector2 (-365,-650), Quaternion.identity, transform);
				productionSlot.GetComponent<ProductionSlot>().SetSlotForUnit(unitPrefab);
				productionSlots.Add(productionSlot);
				position.y -= 60;
			}
		}
	}

	public void ClearCheckMarks()
	{
		foreach (GameObject productionSlot in productionSlots)
		{
			productionSlot.GetComponent<ProductionSlot>().SetCheckMark(false);
		}
	}

}
