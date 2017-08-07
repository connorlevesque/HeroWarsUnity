using System.Collections.Generic;
using UnityEngine;

public class UnitRangeState : InputState {

   public Unit selectedUnit;

   public override void HandleInput(string input, object context) {
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

   public List<Vector2> GetAttackableCoords(Unit unit) {
      List<Vector2> coords = new List<Vector2>();
      bool isArtillery = unit.grouping == UnitGroup.artillery;
      if (isArtillery)
      {
         return GridManager.GetCoordsToAttackHighlight(unit.xy, unit.range);
      } else {
         List<Vector2> movePositions = Pather.GetCoordsToMoveHighlight(unit);
         foreach (Vector2 movePosition in movePositions) {
            foreach (Vector2 attackPosition in GridManager.GetCoordsToAttackHighlight(movePosition, unit.range))
            {
               if (!coords.Contains(attackPosition)) coords.Add(attackPosition);
            }
         }
         return coords;
      }
   }

   public override void Enter() {
      uiManager.ShowRangeUI(GetAttackableCoords(selectedUnit));
   }

   public override void Exit() {
      uiManager.HideRangeUI();
   }

   public UnitRangeState(Unit unit) {
      selectedUnit = unit;
   }
}