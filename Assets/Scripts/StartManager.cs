using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartManager : MonoBehaviour {

	public GameObject newGameBtn;
	public GameObject loadGameBtn;
	public GameObject levelSelectBtn;
	public GameObject instructionsBtn;
	public GameObject quitBtn;

	// Use this for initialization
	void Start () {
		SetUpButtons();
	}
	
	private void SetUpButtons()
	{
		newGameBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadNextLevel());
		loadGameBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadSpecificLevel(GameManager.lastLevelIndex + 2));
		levelSelectBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadSpecificLevel(GameManager.lastLevelIndex));
		instructionsBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadSpecificLevel(GameManager.lastLevelIndex + 1));
		quitBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.Quit());
	}

}
