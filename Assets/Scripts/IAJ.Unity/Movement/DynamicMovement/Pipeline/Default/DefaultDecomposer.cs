using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces;
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
    class PathfindingDecomposer : Decomposer
    {
        public NavMeshPathGraph graph { get; set; }
        public IHeuristic Heuristic { get; set; }

        public override Goal Decompose (KinematicData character, Goal goal)
        {
            AStarPathfinding Astar = new NodeArrayAStarPathFinding(graph, Heuristic);
            Astar.InitializePathfindingSearch(character.position, goal.position);

            // In goal, ends
            if (Astar.StartNode.Equals( Astar.GoalNode)) {
                Debug.Log("Goal Reached");
                return goal;
            }
            // else, plan
            GlobalPath currentSolution;
            Debug.Log("PLANNING");
            if (Astar.InProgress)
            {
                Debug.Log("A star in progress");
                var finished = Astar.Search(out currentSolution, true);
                if (finished && currentSolution != null)
                {
                    // gets first node
                    Debug.Log("getting 1st node");
                    goal.position = currentSolution.PathNodes[0].Position;
                   return goal;
                }
            }
            return goal;      
        }
    }
}
