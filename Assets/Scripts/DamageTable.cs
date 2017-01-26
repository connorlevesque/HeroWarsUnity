using UnityEngine;
using System.Collections;

public class DamageTable {

	private static int[,] table = new int[8,8] {
		//	F	A	S	C	K	G	B	GK
		{  55, 55, 45, 10,  8,  5, 20, 	3 },	// F
		{  65, 65, 55, 20, 35, 15, 30, 10 },	// A
		{  60, 60, 55, 35, 10, 	7, 45, 	5 },	// S
		{  85, 85, 75, 75, 60, 50, 80, 45 },	// C
		{  75, 75, 65, 65, 65, 25, 75, 20 },	// K
		{  70, 70, 65, 50, 65, 55, 60, 50 },	// G
		{  95, 95, 85, 80, 70, 60, 85, 55 },	// B
		{ 105,105, 95, 75, 70, 60, 85, 55 },	// GK
	};

	public static int GetBaseDamage(UnitType attacker, UnitType defender)
	{
		return table[(int)attacker, (int)defender];
	}

}
