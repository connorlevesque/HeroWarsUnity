using UnityEngine;
using System;
using System.Collections.Generic;
using ExtendedListAndArray;

public class ActionState : InputState {

   public Unit selectedUnit;
   private Vector2 origin;
   private DragPathRefiner pathRefiner;

   public override void HandleInput(string input, params object[] context) {
      switch (input) {
         case "tapBlueHighlight": 
            //MoveUnit((Vector3) context);
            break;
         case "draggingUnit":
            UpdatePaths((Vector2) context[1]);
            break;
         case "finishDraggingUnit":
            HandleUnitDragFinished((Unit) context[0]);
            break;
         case "attackBtn":
            TransitionTo(new AbilityState(selectedUnit));
            break;
         case "captureBtn":
            HandleCaptureButton();
            break;
         case "waitBtn":
            Wait();
            break;
         case "backBtn":
            //HandleBackButton();
            break;
         default:
            base.HandleInput(input, context); 
            break;
      }
   }

   private void UpdatePaths(Vector2 mouseGridPosition) {
      pathRefiner.UpdatePaths(mouseGridPosition);
      uiManager.RemoveHighlightPath();
      uiManager.Highlight(pathRefiner.ActionPath, "blue", true);
   }

   private void HandleUnitDragFinished(Unit unit) {
      uiManager.RemoveHighlightPath();
      selectedUnit = unit;
      Vector2 dragPosition = selectedUnit.RoundedTransformPosition;
      bool moving = pathRefiner.MovePositions.Contains(dragPosition) && dragPosition != selectedUnit.xy;
      bool attacking = pathRefiner.AttackPositions.Contains(dragPosition);

      if (moving || attacking) {
         GridManager.MoveUnit(selectedUnit, pathRefiner.Destination);
         if (attacking) {
            Unit targetUnit = GridManager.GetUnit(pathRefiner.Target);
            GridManager.CalculateAttack(selectedUnit, targetUnit);
         }
         selectedUnit.Deactivate();
         TransitionTo(new BaseState());
      } else {
         selectedUnit.transform.position = selectedUnit.xy;
         TransitionBack();
      }
   }

   // private void MoveUnit(Vector2 destination) {
   //    List<Vector2> path = Pather.GetPathToPoint(destination);
   //    InputManager.SetReceiveInput(false);
   //    GridManager.MoveUnitAlongPath(selectedUnit, destination, path, () => { 
   //       CheckActions();
   //       InputManager.SetReceiveInput(true);
   //    });
   //    uiManager.RemoveHighlights();
   // }

   private void HandleCaptureButton() {
      Tile tile = GridManager.GetTile(selectedUnit.xy);
      if (tile.isBuilding) Capture((Building) tile);
   }

   private void Capture(Building building) {
      building.Capture(selectedUnit);
      Wait();
   }

   private void Wait() {
      selectedUnit.Deactivate();
      TransitionTo(new BaseState());
   }

   // private void HandleBackButton() {
   //    Vector2 position = selectedUnit.xy;
   //    bool hasMoved = position != Pather.center;
   //    if (hasMoved) {
   //       UndoMoveUnit(position);
   //    } else {
   //       TransitionBack();
   //    }
   // }

   // private void UndoMoveUnit(Vector2 position) {
   //    GridManager.MoveUnit(position, Pather.center, () => { 
   //       CheckActions();
   //       InputManager.SetReceiveInput(true);
   //    });
   //    this.Enter();
   // }

   public void CheckActions() {
      uiManager.ToggleCaptureBtn(GridManager.CanUnitCapture(selectedUnit));
      uiManager.ToggleAttackBtn(CanUseAttackAction());
   }

   public bool CanUseAttackAction() {
      bool isArtillery = selectedUnit.grouping == UnitGroup.artillery;
      return !(isArtillery && HasMoved());
   }

   public bool HasMoved() {
      Vector2 position = selectedUnit.xy;
      return position != origin;
   }

   public override void Enter() {
      pathRefiner = new DragPathRefiner(selectedUnit);
      uiManager.Highlight(pathRefiner.MovePositions, "blue");
      uiManager.Highlight(pathRefiner.AttackPositions, "red");
   }

   public override void Exit() {
      uiManager.RemoveHighlights();
      uiManager.HideActionUI();
   }

   public ActionState(Unit unit) {
      selectedUnit = unit;
      origin = (Vector2) selectedUnit.xy;
   }
}