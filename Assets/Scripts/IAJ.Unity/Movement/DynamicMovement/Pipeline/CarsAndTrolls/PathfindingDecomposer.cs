using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class PathfindingDecomposer : Decomposer
    {
        public AStarPathfinding Astar { get; set; }

        public override Goal Decompose (KinematicData character, Goal goal)
        {
            if((Astar == null) || (Astar.GoalNode.Position != goal.position)) { 
                Astar = new NodeArrayAStarPathFinding(Graph, Heuristic);
                Astar.InitializePathfindingSearch(character.position, goal.position);
            }

            // In goal, ends
            if (Astar.StartNode.Equals(Astar.GoalNode))
                return goal;

            // else, plan
            GlobalPath currentSolution;
            if (Astar.InProgress)
            {
                var finished = Astar.Search(out currentSolution, true);
                if (finished && currentSolution != null)
                {
                    this.GlobalPath = StringPullingPathSmoothing.SmoothPath(character, currentSolution);
                    this.GlobalPath.CalculateLocalPathsFromPathPositions(character.position);
                    // gets first node
                    goal.position = this.GlobalPath.PathPositions[0];
                   return goal;
                }
            }
            return goal;      
        }
    }
}
