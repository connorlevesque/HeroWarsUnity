using UnityEngine;
using System.Collections;

public class LevelSelectManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void SelectLevel(int lvl)
	{
		GameManager.LoadSpecificLevel(lvl);
	}
	
}
