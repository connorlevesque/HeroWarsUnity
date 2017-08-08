using UnityEngine;
using System.Collections.Generic;
 
public class CameraDrag : MonoBehaviour {

   public float dragSpeed = 8;
   public float mapBuffer;
   
   private Vector3 dragOrigin;
   private float dragStartTime;
   private float dragTimeThreshold = 0.15f;
   private bool stillOverSameUnit = false;
   private bool stayedOverUnit = true;
   private bool draggingUnit = false;
   private Unit tappedUnit;

   private float leftBound;
   private float rightBound;
   private float upperBound;
   private float lowerBound;
 
   void Start() {
    	Camera camera = GetComponent<Camera>();
    	float camSize = camera.orthographicSize;
    	GridManager gM = GameObject.FindWithTag("GridManager").GetComponent<GridManager>();
    	leftBound = camSize * 1.78f - mapBuffer - 0.5f;
    	rightBound = gM.width + mapBuffer - 0.5f - camSize * 1.78f;
    	lowerBound = camSize - mapBuffer - 0.5f;
    	upperBound = gM.height + mapBuffer - 0.5f - camSize;
   }

   void Update() {
      HandleDrag();
   }

   private void HandleDrag() {
      bool tapIsStarting = Input.GetMouseButtonDown(0);
      bool tapIsHeld = Input.GetMouseButton(0);

      if (tapIsStarting) {
         RecordDragStart();
         return;
      } else if (!tapIsHeld) {
         CompleteUnitDrag();
      }

      if (tapIsHeld) {
         if (CanDragUnit()) {
            DragUnit();
         } else {
            MoveCamera();
         }
      }
   }

   private void RecordDragStart() {
      dragOrigin = Input.mousePosition;
      dragStartTime = Time.time;
      tappedUnit = UnitUnderTap();
      stillOverSameUnit = false;
      stayedOverUnit = true;
   }

   private bool CanDragUnit() {
      int currentPlayerIndex = BattleManager.GetCurrentPlayerIndex();
      bool validUnitWasTapped = 
         tappedUnit != null && tappedUnit.activated &&
         currentPlayerIndex == tappedUnit.owner;

      bool sameUnit = tappedUnit == UnitUnderTap();
      float dragTime = Time.time - dragStartTime;
      bool dragThresholdPassed = dragTime > dragTimeThreshold;

      if (sameUnit && dragThresholdPassed) {
         stillOverSameUnit = true;
      }
      if (!sameUnit && !dragThresholdPassed) {
         stayedOverUnit = false;
      }

      return validUnitWasTapped && stillOverSameUnit && stayedOverUnit;
   }

   private void DragUnit() {
      draggingUnit = true;
      GameObject unitGob = tappedUnit.gameObject;
      unitGob.transform.position = MousePosition();
      unitGob.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
      InputManager.HandleInput("draggingUnit", tappedUnit);
   }

   private void CompleteUnitDrag() {
      if (draggingUnit) {
         draggingUnit = false;
         GameObject unitGob = tappedUnit.gameObject;
         unitGob.transform.localScale = new Vector3(1f,1f,1f);
         InputManager.HandleInput("finishDraggingUnit", tappedUnit);
         Debug.Log("Finished dragging unit");
      }
   }

   private void MoveCamera() {
      Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
      Vector3 move = new Vector3(pos.x * dragSpeed * -1, pos.y * dragSpeed * -1, 0);
  
      transform.position += move;
      dragOrigin = Input.mousePosition;

      SnapToBoundaries();
   }

   public void SnapToBoundaries() {
      // check left and right bounds
      if (transform.position.x < leftBound)
      {
         transform.position += new Vector3(leftBound - transform.position.x, 0, 0);
      } else if (transform.position.x > rightBound) {
         transform.position += new Vector3(rightBound - transform.position.x, 0, 0);
      }
      // check upper and lower bounds
      if (transform.position.y < lowerBound)
      {
         transform.position += new Vector3(0, lowerBound - transform.position.y, 0);
      } else if (transform.position.y > upperBound) {
         transform.position += new Vector3(0, upperBound - transform.position.y, 0);
      }
   }

   private Unit UnitUnderTap() {
      foreach (RaycastHit2D hit in GameObjectsUnderTap()) {
         Unit unit = hit.collider.gameObject.GetComponent<Unit>();
         if (unit != null) return unit;
      }
      return null;
   }

   private RaycastHit2D[] GameObjectsUnderTap() {
      RaycastHit2D[] hits = new RaycastHit2D[] {};
      return Physics2D.RaycastAll(MousePosition(), Vector2.zero);
   } 

   private Vector2 MousePosition() {
      Vector3 position3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      return new Vector2(position3D.x, position3D.y);
   }
}
