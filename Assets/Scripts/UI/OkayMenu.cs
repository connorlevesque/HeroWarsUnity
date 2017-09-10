using UnityEngine;
using UnityEngine.UI;
using System;

public class OkayMenu : MonoBehaviour {
  
   public Text message;
   public Button okayBtn;
   public Action onOkay;

   void Start() {
      SetUpButtons();
   }

   private void SetUpButtons() {
      okayBtn.onClick.AddListener(OnOkay);
   }

   private void OnOkay() {
      onOkay();
      Destroy(this.gameObject);
   }
}