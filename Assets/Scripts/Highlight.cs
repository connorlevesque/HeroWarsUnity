using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Highlight : MonoBehaviour {

	public string color;

	void OnMouseUpAsButton() {
		if (!InputManager.IsPointerOverUIButton()) {
			if (color == "blue") {
				InputManager.HandleInput("tapBlueHighlight", transform.position);
			} else if (color == "red") {
				InputManager.HandleInput("tapRedHighlight", transform.position);
			}
		}
	}
}
