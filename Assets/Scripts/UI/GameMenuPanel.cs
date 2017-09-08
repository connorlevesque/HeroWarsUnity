using UnityEngine;
using UnityEngine.UI;

public class GameMenuPanel : MonoBehaviour {

   public Button optionsBtn;
   public Button saveBtn;
   public Button restartBtn;
   public Button quitBtn;
   public Button cancelBtn;

   void Start() {
      SetUpButtons();
   }

   private void SetUpButtons() {
      restartBtn.onClick.AddListener(ShowConfirmRestartMenu);
      quitBtn.onClick.AddListener(ShowConfirmQuitMenu);
      cancelBtn.onClick.AddListener(Close);
   }

   private void ShowConfirmRestartMenu() {
      gameObject.SetActive(false);
      InputManager.UIManager.OpenConfirmMenu("Restart the level?", Restart, Open);
   }

   private void Restart() {
      GameManager.RestartLevel();
   }

   private void ShowConfirmQuitMenu() {
      gameObject.SetActive(false);
      InputManager.UIManager.OpenConfirmMenu("Quit to main menu?", Quit, Open);
   }

   private void Quit() {
      GameManager.LoadSpecificLevel(0);
   }

   public void Open() {
      InputManager.CanReceiveInput = false;
      gameObject.SetActive(true);
   }

   private void Close() {
      gameObject.SetActive(false);
      InputManager.CanReceiveInput = true;
   }
}