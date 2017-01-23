using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour {

	public string color;

	void OnMouseUpAsButton()
	{
		if (color == "blue")
		{
			InputManager.MoveHighlightClicked(transform.position);
		} else if (color == "red") {
			InputManager.AttackHighlightClicked(transform.position);
		}
	}
}
