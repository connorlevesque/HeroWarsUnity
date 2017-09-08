using UnityEngine;
using System.Collections;

public class InputState {

   public UIManager uiManager;

   public virtual void HandleInput(string input, params object[] context) {
      switch (input) {
         case "endTurnBtn":
            InputManager.ChangeTurns();
            break;
         case "gameMenuBtn":
            TransitionTo(new GameMenuState());
            break;
         default: 
            // Debug.LogFormat("Unknown input '{0}' passed to HandleInput()", input);
            break;
      }
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