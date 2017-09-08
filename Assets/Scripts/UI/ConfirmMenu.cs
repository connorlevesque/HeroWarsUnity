using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmMenu : MonoBehaviour {
  
   public Text message;
   public Button yesBtn;
   public Button noBtn;

   public Action onYes;
   public Action onNo;

   void Start() {
      SetUpButtons();
   }

   private void SetUpButtons() {
      yesBtn.onClick.AddListener(OnYes);
      noBtn.onClick.AddListener(OnNo);
   }

   private void OnYes() {
      onYes();
   }

   private void OnNo() {
      onYes();
   }
}