using UnityEngine;
using System.Collections;

public class InputState {

   public UIManager uiManager;

   public virtual void HandleInput(string input, params object[] context) {
      switch (input) {
         case "endTurnBtn":
            OpenConfirmEndTurnMenu();
            break;
         case "gameMenuBtn":
            OpenGameMenuPanel();
            break;
         default: 
            // Debug.LogFormat("Unknown input '{0}' passed to HandleInput()", input);
            break;
      }
   }

   private void OpenConfirmEndTurnMenu() {
      uiManager.OpenConfirmMenu("End your turn?", InputManager.ChangeTurns, ()=>{});
   }

   private void OpenGameMenuPanel() {
      GameMenuPanel gameMenuPanel = uiManager.gameMenu.GetComponent<GameMenuPanel>();
      gameMenuPanel.Open();
   }

   public virtual void Enter() {}
   public virtual void Exit() {}

   protected void TransitionTo(InputState newState) {
      InputManager.TransitionTo(newState);
   }

   protected void TransitionBack() {
      InputManager.TransitionBack();
   }

   public InputState() {
      uiManager = InputManager.UIManager;
   }

   public string Name() {
      return this.GetType().Name;
   }
}