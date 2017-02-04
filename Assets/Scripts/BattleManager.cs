using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerType { none, local, online, computer };

public class BattleManager : MonoBehaviour {

	private static BattleManager instance;

	public int totalPlayers = 2;
	private int maxPlayers = 2;
	private int currentPlayerIndex = 1;
	private Player[] players;
	private string[] colors = new string[] {"gray", "red", "blue"};
	private PlayerType[] playerTypes = new PlayerType[] 
		{PlayerType.none, PlayerType.local, PlayerType.computer};
	private int turn = 1;
	private int[] funds;

	// change model methods
	public static void StartTurn()
	{
		int totalRevenue = 0;
		List<Building> friendlyBuildings = GridManager.GetFriendlyBuildings();
		foreach (Building building in friendlyBuildings)
		{
			totalRevenue += building.revenue;
		}
		instance.funds[GetCurrentPlayerIndex()] += totalRevenue;
	}

	public static void EndTurn()
	{
		instance.turn++;
		instance.currentPlayerIndex = 
			(instance.currentPlayerIndex % instance.totalPlayers) + 1;
	}

	// property accessors
	public static int GetCurrentPlayerIndex()
	{
		return instance.currentPlayerIndex;
	}

	public static int GetNextPlayerIndex()
	{
		return (instance.currentPlayerIndex % instance.totalPlayers) + 1;
	}

	public static int GetPlayerIndexAfter(int player)
	{
		return (player % instance.totalPlayers) + 1;
	}

	public static PlayerType GetCurrentPlayerType()
	{
		return instance.playerTypes[GetCurrentPlayerIndex()];
	}

	public static int GetFundsForCurrentPlayer()
	{
		return instance.funds[instance.currentPlayerIndex];
	}

	public static void ChangeFunds(int amount)
	{
		instance.funds[GetCurrentPlayerIndex()] += amount;
	}

	// initialization
	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		SetUpPlayers();
		SetUpFunds();
		StartTurn();
	}

	private void SetUpPlayers()
	{
		players = new Player[totalPlayers + 1];
		players[0] = null;
		if (totalPlayers > maxPlayers) totalPlayers = maxPlayers;
		for (int i = 1; i <= totalPlayers; i++)
		{
			Player player = new Player(colors[i], playerTypes[i]);
			players[i] = player;
		}
	}

	private void SetUpFunds()
	{
		funds = new int[totalPlayers + 1];
		for (int i = 0; i < funds.Length; i++)
		{
			funds[i] = 0;
		}
	}
}
