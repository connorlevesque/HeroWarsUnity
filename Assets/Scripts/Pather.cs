using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pather : MonoBehaviour {

	public static List<Vector2> GetCoordsToMoveHighlight(Unit unit)
	{
		List<Vector2> coords = new List<Vector2>();
		Vector2 center = unit.transform.position;
		coords.Add(center);
		coords.Add(center + Vector2.up);
		coords.Add(center + Vector2.down);
		coords.Add(center + Vector2.left);
		coords.Add(center + Vector2.right);
		return coords;
	}

}
