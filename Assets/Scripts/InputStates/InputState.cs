using UnityEngine;
using System.Collections;

public class InputState {

   public UIManager uiManager;

   public virtual void HandleInput(string input, object context) {
      switch (input) {
         case "backBtn":
            TransitionBack();
            break;
         default: 
            Debug.LogFormat("Unknown input '{0}' passed to HandleInput()", input);
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
      uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
   }

   public string Name() {
      return this.GetType().Name;
   }
}