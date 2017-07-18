
public class GameOverState : InputState {

   private string menuType;

   public override void HandleInput(string input, object context) {
      switch (input) {
         case "okayBtn":
            CompleteLevel();
            break;
         case "drag_unit":
            // handle
            break;
         case "tap_production":
            // handle
            break;
         default:
            base.HandleInput(input, context); 
            break;
      }
   }

   public void CompleteLevel() {
      if (menuType == "win") {
         GameManager.LoadNextLevel();
      } else if (menuType == "lose") {
         GameManager.RestartLevel();
      }
   }

   public override void Enter() {
      string message = "";
      if (menuType == "win") {
         message = "You won!";
      } else if (menuType == "lose") {
         message = "You lost!";
      }
      uiManager.ShowWinLoseUI(message);
   }

   public override void Exit() {
      uiManager.HideWinLoseUI();
   }

   public GameOverState(string type) {
      menuType = type;
   }
}