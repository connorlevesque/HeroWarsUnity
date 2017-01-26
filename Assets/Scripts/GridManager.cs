using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	private static GridManager instance;

	public int width;
	public int height;
	public GameObject[] startingTiles;
	public GameObject[] startingUnits;
	public GameObject[] unitPrefabs;

	private Tile[,] tiles;
	private Dictionary<Vector2,Unit> units = new Dictionary<Vector2,Unit>();
 
	// grid data accessors
	public static int Width() {
		return instance.width;
	}

	public static int Height() {
		return instance.height;
	}

	// tile accessors
	public static Tile GetTile(Vector2 position) {
		return instance.tiles[(int)position.x,(int)position.y];
	}

	public static Tile[,] GetTiles() {
		return instance.tiles;
	}

	public static List<Building> GetBuildings()
	{
		List<Building> buildings = new List<Building>();
		foreach(Tile tile in instance.tiles)
		{
			if (tile.isBuilding)
			{
				buildings.Add((Building)tile);
			}
		}
		return buildings;
	}

	public static List<Building> GetFriendlyBuildings()
	{
		List<Building> buildings = new List<Building>();
		foreach (Tile tile in instance.tiles)
		{
			if (tile.isBuilding)
			{
				Building building = (Building)tile;
				if (building.owner == BattleManager.GetCurrentPlayerIndex())
				{
					buildings.Add((Building)tile);	
				}
			}
		}
		return buildings;
	}

	public static List<Building> GetEnemyBuildings()
	{
		List<Building> buildings = new List<Building>();
		foreach(Tile tile in instance.tiles)
		{
			if (tile.isBuilding)
			{
				Building building = (Building)tile;
				if (building.owner != BattleManager.GetCurrentPlayerIndex())
				{
					buildings.Add((Building)tile);	
				}
			}
		}
		return buildings;
	}

	public static List<Vector2> GetCoordsToAttackHighlight(Unit attackingUnit)
	{
		List<Vector2> coords = new List<Vector2>();
		for (int x = 0; x < Width(); x++) {
			for (int y = 0; y < Height(); y++) {
				int dx = (int)Mathf.Abs(attackingUnit.transform.position.x - x);
				int dy = (int)Mathf.Abs(attackingUnit.transform.position.y - y);
				if (attackingUnit.range[0] <= dx + dy && dx + dy <= attackingUnit.range[1])
				{
					coords.Add(new Vector2(x,y));
				}
			}
		}
		return coords;
	}

	public static bool CanUnitCapture(Unit unit)
	{
		Tile tile = GetTile(unit.transform.position);
		if (tile.isBuilding) {
			Building building = (Building)tile;
			if (unit.grouping == UnitGroup.infantry && building.owner != unit.owner) {
				return true;
			}
		}
		return false;
	}

	// unit accessors
	public static Unit GetUnit(Vector2 position) {
		return instance.units[position];
	}

	public static Dictionary<Vector2,Unit> GetUnits() {
		return instance.units;
	}

	public static Dictionary<Vector2,Unit> GetFriendlyUnits() {
		Dictionary<Vector2,Unit> friendlyUnits = new Dictionary<Vector2,Unit>();
		foreach (KeyValuePair<Vector2,Unit> pair in instance.units)
		{
			if (pair.Value.owner == BattleManager.GetCurrentPlayerIndex())
			{
				friendlyUnits.Add(pair.Key,pair.Value);
			}
		}
		return friendlyUnits;
	}

	public static Dictionary<Vector2,Unit> GetEnemyUnits() {
		Dictionary<Vector2,Unit> enemyUnits = new Dictionary<Vector2,Unit>();
		foreach (KeyValuePair<Vector2,Unit> pair in instance.units)
		{
			if (pair.Value.owner != BattleManager.GetCurrentPlayerIndex())
			{
				enemyUnits.Add(pair.Key,pair.Value);
			}
		}
		return enemyUnits;
	}

	public static List<UnitType> GetUnitTypes() {
		List<UnitType> unitTypes = new List<UnitType>();
		foreach (GameObject prefab in instance.unitPrefabs)
		{
			unitTypes.Add(prefab.GetComponent<Unit>().type);
		}
		return unitTypes;
	}

	// unit prefab accessors
	public static GameObject GetUnitPrefab(UnitType type) 
	{
		foreach (GameObject prefab in instance.unitPrefabs)
		{
			if (prefab.GetComponent<Unit>().type == type)
			{
				return prefab;
			}
		}
		Debug.LogFormat("Error: cannot find unit prefab of type '{0}'", type.ToString());
		return null;
	}

	public static GameObject[] GetUnitPrefabs()
	{
		return instance.unitPrefabs;
	}

	// change grid methods
	public static void UpdateUnit(Unit unit)
	{
		instance.units[unit.transform.position] = unit;
	}

	public static void UpdateBuilding(Building building)
	{
		instance.tiles[(int)building.transform.position.x,
					   (int)building.transform.position.y] = building;
	}

	public static void DestroyUnit(Vector2 position)
	{
		Unit unit = instance.units[position];
		instance.units.Remove(position);
		Destroy(unit.gameObject);
	}

	public static void AddUnit(GameObject unitPrefab, Vector2 position)
	{
		GameObject unitGM = (GameObject)Instantiate(unitPrefab, position, Quaternion.identity);
		unitGM.GetComponent<Unit>().Deactivate();
		instance.units.Add(position, unitPrefab.GetComponent<Unit>());
	}

	public static void MoveUnitAlongPath(Unit unit, Vector2 destination, List<Vector2> path)
	{
		instance.StartCoroutine(MoveUnitAlongPathCoroutine(unit, destination, path));
	}

	private static IEnumerator MoveUnitAlongPathCoroutine(Unit unit, Vector2 destination, List<Vector2> path)
	{
		Vector2 startPosition = unit.transform.position;
		foreach (Vector2 direction in path)
		{
			int frames = 10;
			for (int i = 0; i < frames; i++) {
				unit.transform.position += (Vector3)(direction / frames);
				yield return new WaitForSeconds(.05f / frames);
			}
		}
		MoveUnit(startPosition, destination);
		InputManager.SetReceiveInput(true);
	}

	public static void MoveUnit(Vector2 a, Vector2 b)
	{
		Unit unit = GetUnit(a);
		instance.units.Remove(a);
		instance.units.Add(b,unit);
		unit.transform.position = b;
		InputManager.CheckActions();
	}

	public static void ActivateUnits()
	{
		foreach (Unit unit in GetFriendlyUnits().Values)
		{
			unit.Activate();
		}
	}

	public static void ShowDamageLabels(List<Vector2> positions, Unit attackingUnit)
	{
		foreach (Vector2 position in positions)
		{
			if (instance.units.ContainsKey(position))
			{
				Unit targetUnit = instance.units[position];
				if (Combat.CanAttack(attackingUnit, targetUnit))
				{
					int damage = Combat.GetFinalDamage(attackingUnit, targetUnit, false);
					targetUnit.ShowDamageLabel(damage);
				}
			}
		}
	}

	public static void HideDamageLabels()
	{
		foreach (Unit unit in GetUnits().Values)
		{
			unit.HideDamageLabel();
		}
	}

	public static void CalculateAttack(Unit attacker, Unit defender)
	{
		Unit[] participants = Combat.CalculateCombatForUnits(attacker, defender);
		Debug.LogFormat("attacker.health = {0}", participants[0].health);
		Debug.LogFormat("defender.health = {0}", participants[1].health);
		if (attacker.health <= 0)
		{
			DestroyUnit(attacker.transform.position);
		}
		if (defender.health <= 0)
		{
			DestroyUnit(defender.transform.position);
		}
	}

	public static void RefreshControlPoints()
	{
		List<Building> buildings = GetBuildings();
		foreach (Building building in buildings)
		{
			if (instance.units.ContainsKey(building.transform.position))
			{
				if (building.owner == instance.units[building.transform.position].owner)
				{
					building.RefreshControlPoints();
				}
			} else {
				building.RefreshControlPoints();
			}
		}
	}

	// initialization
	void Awake() {
		instance = this;
		tiles = new Tile[width, height];
		ParseStartingTiles();
		ParseStartingUnits();
		OrderUnitPrefabs();
	}

	void Start () {

	}

	private void ParseStartingTiles()
	{
		int x;
		int y;
		foreach(GameObject element in startingTiles)
		{
			if (element.GetComponent<Tile>())
			{
				Tile tile = element.GetComponent<Tile>();
				x = (int)tile.transform.position.x;
				y = (int)tile.transform.position.y;
				instance.tiles[x,y] = tile;
			}
		}
	}

	private void ParseStartingUnits()
	{
		int x;
		int y;
		foreach(GameObject element in startingUnits)
		{
			if (element.GetComponent<Unit>())
			{
				Unit unit = element.GetComponent<Unit>();
				x = (int)unit.transform.position.x;
				y = (int)unit.transform.position.y;
				instance.units.Add(new Vector2(x,y), unit);
			}
		}
	}

	private void OrderUnitPrefabs()
	{
		for (int i = 0; i < unitPrefabs.Length - 1; i++) {
			GameObject cheapestUnitPrefab = unitPrefabs[i];
			int k = i;
			for (int j = i + 1; j < unitPrefabs.Length; j++)
			{
				GameObject nextUnitPrefab = unitPrefabs[j];
				if (nextUnitPrefab.GetComponent<Unit>().cost < 
					cheapestUnitPrefab.GetComponent<Unit>().cost)
				{
					cheapestUnitPrefab = nextUnitPrefab;
					k = j;
				}
			}
			if (k != i)
			{
				GameObject temp = unitPrefabs[i];
				unitPrefabs[i] = cheapestUnitPrefab;
				unitPrefabs[k] = temp;
			}
		}
		instance.unitPrefabs = unitPrefabs;
	}
}
