using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    public class DefaultDecomposer : Decomposer
    {
        public override Goal Decompose (KinematicData character, Goal goal)
        {
            AStarPathfinding Astar = new NodeArrayAStarPathFinding(Graph, Heuristic);
            Astar.InitializePathfindingSearch(character.position, goal.position);

            // In goal, ends
            if (Astar.StartNode.Equals(Astar.GoalNode)) {
                return goal;
            }
            // else, plan
            GlobalPath currentSolution;
            if (Astar.InProgress)
            {
                var finished = Astar.Search(out currentSolution, true);
                if (finished && currentSolution != null)
                {
                    // gets first node
                    goal.position = currentSolution.PathNodes[0].Position;
                   return goal;
                }
            }
            return goal;      
        }
    }
}
