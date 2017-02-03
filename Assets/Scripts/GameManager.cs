using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static int lastLevelIndex = 3;
	private int currentScene;

	void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if(instance != this) Destroy(gameObject);
    }

    void Start() {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	// Loads the next scene
    public static void LoadNextLevel() {
        instance.StopAllCoroutines();
        instance.currentScene++;
        if(instance.currentScene == lastLevelIndex) instance.currentScene = 0;
        SceneManager.LoadScene(instance.currentScene);
    }

    // Loads the next scene
    public static void LoadSpecificLevel(int lvl) {
        instance.StopAllCoroutines();
        instance.currentScene = lvl;
        SceneManager.LoadScene(instance.currentScene);
    }

    // Reloads the current scene
    public static void RestartLevel() {
        instance.StopAllCoroutines();
        SceneManager.LoadScene(instance.currentScene);
    }

    // Returns to the first scene
    public static void Exit() {
        instance.currentScene = 0;
        SceneManager.LoadScene(instance.currentScene);
    }

    // Quits the game entirely
    public static void Quit() {
        Application.Quit();
    }

}
