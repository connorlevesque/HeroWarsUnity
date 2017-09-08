
public class ConfirmChangeSceneState : InputState {

   private string menuType;

   public override void HandleInput(string input, params object[] context) {
      switch (input) {
         case "yesBtn":
            ChangeScene();
            break;
         case "noBtn":
            TransitionTo(new BaseState());
            break;
         default:
            base.HandleInput(input, context); 
            break;
      }
   }

   public void ChangeScene() {
      if (menuType == "restart") {
         GameManager.RestartLevel();
      } else if (menuType == "quit") {
         GameManager.LoadSpecificLevel(0);
      }
   }

   public override void Enter() {
      string message = "";
      if (menuType == "restart") {
         message = "Are you sure you want to restart the level?";
      } else if (menuType == "quit") {
         message = "Are you sure you want to quit to the start screen?";
      }
      uiManager.ShowChangeSceneUI(message);
   }

   public override void Exit() {
      uiManager.HideChangeSceneUI();
   }

   public ConfirmChangeSceneState(string type) {
      menuType = type;
   }
}