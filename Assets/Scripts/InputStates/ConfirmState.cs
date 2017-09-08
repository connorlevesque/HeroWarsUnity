
public class ConfirmAbilityState : InputState {

   public override void HandleInput(string input, params object[] context) {
      switch (input) {
         case "tap_unit":
            // handle
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

   public override void Enter() {
      uiManager.ShowConfirmUI();
   }

   public override void Exit() {
      uiManager.HideConfirmUI();
   }
}