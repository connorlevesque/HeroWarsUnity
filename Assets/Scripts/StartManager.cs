using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartManager : MonoBehaviour {

	public GameObject newGameBtn;
	//public GameObject loadGameBtn;
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
		//loadGameBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadSpecificLevel(GameManager.winScreenIndex + 3));
		levelSelectBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadSpecificLevel(GameManager.winScreenIndex + 1));
		instructionsBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadSpecificLevel(GameManager.winScreenIndex + 2));
		quitBtn.GetComponent<Button>().onClick.AddListener(() => GameManager.Quit());
	}

}
