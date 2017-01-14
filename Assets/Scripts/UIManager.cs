using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public GameObject highlightBlue;
	public GameObject highlightRed;

	public void HighlightMoveTiles(List<Vector2> coords)
	{
		foreach(Vector2 coord in coords)
		{
			Instantiate(highlightBlue, coord, Quaternion.identity);
		}
	}

}
