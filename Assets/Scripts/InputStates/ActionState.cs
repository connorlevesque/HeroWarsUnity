using UnityEngine;
using System;
using System.Collections.Generic;

public class ActionState : InputState {

   public Unit selectedUnit;
   private Vector2 origin;

   public override void HandleInput(string input, object context) {
      switch (input) {
         case "tapBlueHighlight": 
            MoveUnit((Vector3) context);
            break;
         case "finishDraggingUnit":
            HandleUnitDragFinished((Unit) context);
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
            HandleBackButton();
            break;
         default:
            base.HandleInput(input, context); 
            break;
      }
   }

   private void HandleUnitDragFinished(Unit unit) {
      Vector2 unitDraggedPosition = unit.RoundedTransformPosition;
      bool validPlaceToMove = uiManager.IsPointHighlighted(unitDraggedPosition);
      if (validPlaceToMove) {
         GridManager.MoveUnit(unit, unitDraggedPosition);
      } else {
         unit.transform.position = unit.xy;
         TransitionBack();
      }
      uiManager.RemoveHighlights();
   }

   private void MoveUnit(Vector2 destination) {
      List<Vector2> path = Pather.GetPathToPoint(destination);
      InputManager.SetReceiveInput(false);
      GridManager.MoveUnitAlongPath(selectedUnit, destination, path, () => { 
         CheckActions();
         InputManager.SetReceiveInput(true);
      });
      uiManager.RemoveHighlights();
   }

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

   private void HandleBackButton() {
      Vector2 position = selectedUnit.xy;
      bool hasMoved = position != Pather.center;
      if (hasMoved) {
         UndoMoveUnit(position);
      } else {
         TransitionBack();
      }
   }

   private void UndoMoveUnit(Vector2 position) {
      GridManager.MoveUnit(position, Pather.center, () => { 
         CheckActions();
         InputManager.SetReceiveInput(true);
      });
      this.Enter();
   }

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
      List<Vector2> coords = new List<Vector2>();
      if (!HasMoved()) {
         coords = Pather.GetCoordsToMoveHighlight(selectedUnit);
         bool unitIsHighlighted = coords.Contains(selectedUnit.xy);
         if (unitIsHighlighted) coords.Remove(selectedUnit.xy);
      }
      bool canCapture = GridManager.CanUnitCapture(selectedUnit);
      uiManager.ShowActionUI(coords, canCapture, CanUseAttackAction(), false, false);
   }

   public override void Exit() {
      uiManager.HideActionUI();
   }

   public ActionState(Unit unit) {
      selectedUnit = unit;
      origin = (Vector2) selectedUnit.xy;
   }
}