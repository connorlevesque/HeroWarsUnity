using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour {

	public string color;

	void OnMouseUpAsButton()
	{
		InputManager.HighlightClicked(this.color);
	}
}
