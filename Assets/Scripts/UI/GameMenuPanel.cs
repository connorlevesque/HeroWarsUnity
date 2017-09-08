using UnityEngine;
using UnityEngine.UI;

public class GameMenuPanel : MonoBehaviour {

   public Button optionsBtn;
   public Button saveBtn;
   public Button restartBtn;
   public Button quitBtn;
   public Button cancelBtn;
   
   public GameObject confirmMenuPrefab;
   private GameObject confirmMenuGob;

   void Start() {
      SetUpButtons();
   }

   private void SetUpButtons() {
      restartBtn.onClick.AddListener(ShowConfirmRestartMenu);
      quitBtn.onClick.AddListener(ShowConfirmQuitMenu);
      cancelBtn.onClick.AddListener(Cancel);
   }

   private void ShowConfirmRestartMenu() {
      gameObject.SetActive(false);
      Transform parent = InputManager.UIManager.transform;
      confirmMenuGob = Instantiate(confirmMenuPrefab, parent);
      ConfirmMenu confirmMenu = confirmMenuGob.GetComponent<ConfirmMenu>();
      confirmMenu.message.text = "Are you sure you want to restart the level?";
      confirmMenu.onYes = Restart;
      confirmMenu.onNo = HideConfirmMenu;
   }

   private void Restart() {

   }

   private void ShowConfirmQuitMenu() {
      gameObject.SetActive(false);
      Transform parent = InputManager.UIManager.transform;
      confirmMenuGob = Instantiate(confirmMenuPrefab, parent);
      ConfirmMenu confirmMenu = confirmMenuGob.GetComponent<ConfirmMenu>();
      confirmMenu.message.text = "Are you sure you want to quit the level?";
      confirmMenu.onYes = Quit;
      confirmMenu.onNo = HideConfirmMenu;
   }

   private void Quit() {
      
   }

   private void HideConfirmMenu() {
      Destroy(confirmMenuGob);
      confirmMenuGob = null;
      gameObject.SetActive(true);
   }

   private void Cancel() {
      gameObject.SetActive(false);
      InputManager.CanReceiveInput = true;
   }
}