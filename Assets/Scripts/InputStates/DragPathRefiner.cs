using UnityEngine;
using System;
using System.Collections.Generic;

public class DragPathRefiner {
  
   private Unit walker;
   private UnitPather pather;
   private Vector2 lastStep;
   private Vector2 target;
   private List<Vector2> movePositions = new List<Vector2>();
   private List<Vector2> attackPositions = new List<Vector2>();
   private List<Vector2> dragPath = new List<Vector2>();
   private List<Vector2> actionPath = new List<Vector2>();

   public List<Vector2> MovePositions { get { return movePositions; } }
   public List<Vector2> AttackPositions { get { return attackPositions; } }
   public List<Vector2> ActionPath { get { return actionPath; } }
   public Vector2 Destination { get { return actionPath[actionPath.Count-1]; } }
   public Vector2 Target { get { return target; } }

   private bool spew = true;

   public DragPathRefiner(Unit walker) {
      this.walker = walker;
      pather = new UnitPather(walker);
      movePositions = pather.MovePositions();
      attackPositions = pather.AttackPositions(movePositions);

      dragPath.Add(walker.xy);
      actionPath.Add(walker.xy);
      lastStep = walker.xy;
   }

   public void UpdatePaths(Vector2 newStep) {
      bool onNewTile = newStep != lastStep;
      if (onNewTile) {
         if (spew) Debug.Log("Updating path:");
         dragPath.Add(newStep);
         lastStep = newStep;
         actionPath = FindActionPath();
         if (spew) Debug.LogFormat("dragPath = {0}", dragPath.ToString());
         if (spew) Debug.LogFormat("actionPath = {0}", actionPath.ToString());

         bool beyondActionRange = !attackPositions.Contains(newStep) && 
                                    !movePositions.Contains(newStep);
         bool tooLong = dragPath.Count > walker.movePoints + walker.range[0] + 1;
         bool atOrigin = newStep == walker.xy;
         if (beyondActionRange || tooLong || atOrigin) {
            dragPath = actionPath;
            if (spew) Debug.Log("dragPath updated to actionPath");
         }
      }
   }

   private List<Vector2> FindActionPath() {
      for (int i = dragPath.Count-1; i >= 0; i--) {
         Vector2 step = dragPath[i];

         if (attackPositions.Contains(step)) {
            int index = dragPath.IndexOf(step);
            List<Vector2> tentPath = dragPath.GetRange(0, index+1);
            return FindAttackPath(tentPath);

         } else if (movePositions.Contains(step)) {
            int index = dragPath.IndexOf(step);
            List<Vector2> tentPath = dragPath.GetRange(0, index+1);
            return FindMovePath(tentPath);
         }
      }
      string message = "Could not find actionPath for dragPath";
      throw new ArgumentException(message, dragPath.ToString());
   }

   private List<Vector2> FindAttackPath(List<Vector2> tentPath) { // tentPath = tentative path
      if (spew) Debug.Log("Finding Attack Path");
      List<Vector2> movePath = new List<Vector2>();
      Vector2 destination = DestinationFromAttackPath(tentPath);
      if (spew) Debug.LogFormat("Destination = {0}", destination.ToString());
      if (tentPath.Contains(destination)) {
         if (spew) Debug.Log("Destination in tentPath");
         int index = tentPath.LastIndexOf(destination);
         movePath = FindMovePath(tentPath.GetRange(0, index+1));
      } else {
         if (spew) Debug.Log("Destination not in tentPath, finding new path from trace");
         movePath = pather.TracePathTo(destination);
      }
      return movePath;
   }

   private List<Vector2> FindMovePath(List<Vector2> tentPath) { // tentPath = tentative path
      if (spew) Debug.Log("Finding Move Path");
      if (pather.CanTakePath(tentPath)) {
         if (spew) Debug.Log("Taking tentPath");
         return tentPath;
      } else {
         if (spew) Debug.Log("Cannot take tentPath, finding new path from trace");
         Vector2 destination = tentPath[tentPath.Count - 1];
         return pather.TracePathTo(destination);
      }  
   }

   private Vector2 DestinationFromAttackPath(List<Vector2> tentPath) { // tentPath = tentative path
      Vector2 badDestination = new Vector2(-1000,-1000);
      Vector2 destination = badDestination;
      Vector2 backup = badDestination;
      target = tentPath[tentPath.Count - 1];
      foreach (Vector2 attackOrigin in pather.AttackOrigins(target)) {
         if (movePositions.Contains(attackOrigin)) {
            if (tentPath.Contains(attackOrigin)) {
               if (destination == badDestination) {
                  bool later = tentPath.LastIndexOf(attackOrigin) > tentPath.LastIndexOf(destination);
                  if (later) destination = attackOrigin;
               } else {
                  destination = attackOrigin;
               }
            } else {
               backup = attackOrigin;
            }
         }
      }
      if (destination != badDestination) return destination;
      if (backup != badDestination) return backup;
      string message = "Could not find destination for tentPath";
      throw new ArgumentException(message, tentPath.ToString());
   }
}