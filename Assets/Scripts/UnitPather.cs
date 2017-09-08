using UnityEngine;
using System;
using System.Collections.Generic;

public class UnitPather {

   private static Vector2[] directions = new Vector2[4] 
      { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

   private Unit unit;
   private Vector2 origin;
   private List<Vector2> rangeVectors;
   private Node[,] nodes;

   public List<Vector2> MovePositions() {
      List<Vector2> positions = new List<Vector2>();
      SetupNodes();
      Queue<Node> steps = new Queue<Node>();
      positions.Add(unit.xy);
      steps.Enqueue(NodeAt(unit.xy));
      while (steps.Count > 0) {
         Node u = steps.Dequeue();

         foreach (Vector2 direction in directions) {
            Vector2 vxy = u.position + direction;
            if (InBounds(vxy)) {
               Node v = NodeAt(vxy);
               int newPathCost = u.pathCost + v.moveCost;
               if (newPathCost < v.pathCost && newPathCost <= unit.movePoints) {

                  v.pathCost = newPathCost;
                  v.trace = direction;
                  bool unique = !positions.Contains(v.position);
                  if (unique && !v.friendly) positions.Add(v.position);
                  steps.Enqueue(v);
               }
            }
         }
      }
      return positions;
   }

   public List<Vector2> AttackPositions(List<Vector2> movePositions) {
      List<Vector2> attackPositions = new List<Vector2>();
      foreach (Vector2 movePosition in movePositions) {
         foreach (Vector2 rangeVector in RangeVectors()) {
            Vector2 target = movePosition + rangeVector;
            bool enemyPresent = GridManager.GetEnemyUnits().ContainsKey(target);
            bool alreadyAdded = attackPositions.Contains(target);
            if (enemyPresent && !alreadyAdded && InBounds(target)) attackPositions.Add(target);
         }
      }
      return attackPositions;
   }

   public List<Vector2> AttackOrigins(Vector2 target) {
      List<Vector2> attackOrigins = new List<Vector2>();
      foreach (Vector2 rangeVector in RangeVectors()) {
         attackOrigins.Add(target - rangeVector);
      }
      return attackOrigins;
   }

   private List<Vector2> RangeVectors() {
      if (rangeVectors == null) {
         rangeVectors = new List<Vector2>();
         int lower = unit.range[0] * -1;
         int upper = unit.range[0];
         for (int x = lower; x < upper; x++) {
            for (int y = lower; y < upper; y++) {
               int dx = (int)Mathf.Abs(x);
               int dy = (int)Mathf.Abs(y);
               if (unit.range[0] == dx + dy) rangeVectors.Add(new Vector2(x,y));
            }
         }
      }
      return rangeVectors;
   }

   public List<Vector2> BehaviorMovePositions(bool attacking)
   {
      List<Vector2> movePositions = new List<Vector2>();
      if (unit.behaviour == Behaviour.hold || 
         (unit.behaviour == Behaviour.defend && !attacking) ||
         (unit.grouping == UnitGroup.artillery && attacking))
      {
         movePositions.Add(unit.xy);
      } else {
         movePositions = MovePositions();
      }
      return movePositions;
   }

   public void SetupNodes() {
      if (unit.xy != origin) {
         origin = unit.xy;
         nodes = new Node[GridManager.Width(), GridManager.Height()];

         for (int x = 0; x < GridManager.Width(); x++) {
            for (int y = 0; y < GridManager.Height(); y++) {
               Vector2 position = new Vector2(x,y);
               Tile tile = GridManager.GetTile(position);
               int moveCost = unit.moveCosts[tile.type];
               nodes[x,y] = new Node(position, moveCost);
            }
         }

         foreach (KeyValuePair<Vector2,Unit> pair in GridManager.GetUnits()) {
            Unit other = pair.Value;
            Node u = NodeAt(other.xy);
            u.friendly = other.owner == unit.owner;
            if (!u.friendly) u.moveCost = 1000;
         }

         Node center = NodeAt(origin);
         center.pathCost = 0;
         center.friendly = false;
      }
   }

   public List<Vector2> TracePathTo(Vector2 destination) {
      MovePositions(); // set trace
      List<Vector2> path = new List<Vector2>();
      path.Add(destination);
      int movePoints = unit.movePoints;
      while (path[0] != origin && movePoints > 0) {
         Vector2 current = path[0];
         Vector2 next = current - NodeAt(current).trace;
         movePoints -= NodeAt(current).moveCost;
         path.Insert(0,next);
      }
      return path;
   }

   public bool CanTakePath(List<Vector2> path) {
      return CostOfPath(path) <= unit.movePoints;
   }

   public int CostOfPath(List<Vector2> path) {
      SetupNodes();
      int moveCost = 0;
      for (int i = 1; i < path.Count; i++) {
         Vector2 p = path[i];
         bool contiguous = (p - path[i-1]).magnitude == 1;
         if (!contiguous) return 1000;
         moveCost += NodeAt(p).moveCost;
      }
      return moveCost;
   }

   public Node NodeAt(int x, int y) {
      Vector2 p = new Vector2(x,y);
      return NodeAt(p);
   }

   public Node NodeAt(Vector2 p) {
      SetupNodes();
      return nodes[(int)p.x,(int)p.y];
   }

   public static bool InBounds(int x, int y) {
      Vector2 p = new Vector2(x,y);
      return InBounds(p);
   }

   public static bool InBounds(Vector2 p) {
      return (p.x >= 0) && (p.x < GridManager.Width()) &&
             (p.y >= 0) && (p.y < GridManager.Height());
   }
  
   public UnitPather(Unit unit) {
      this.unit = unit;
      SetupNodes();
   }
}


public class Node {

   public Vector2 position = new Vector2();
   public int moveCost;
   public int pathCost = 1000;
   public Vector2 trace;
   public bool friendly = false;

   public void Log() {
      Debug.LogFormat("({0},{1}) pathCost={2}, trace=({3},{4})", 
                      position.x, position.y, pathCost, trace.x, trace.y);
   }

   public Node(Vector2 position, int moveCost) {
      if (moveCost < 1) throw new ArgumentException("Node moveCost cannot be < 1");
      this.position = position;
      this.moveCost = moveCost;
   }
}