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
        public AStarPathfinding Astar { get; set; }
        public GlobalPath AStarSolution { get; set; }
        public float CurrentParam  {get;set;}

        public PathfindingDecomposer()
        {
            CurrentParam = 0.0f;
        }

        public override Goal Decompose (KinematicData character, Goal goal)
        {
            if((Astar == null) || (Astar.GoalNode.Position != goal.position)) { 
                Astar = new NodeArrayAStarPathFinding(Graph, Heuristic);
                Astar.InitializePathfindingSearch(character.position, goal.position);
                CurrentParam = 0.0f;
                Debug.Log("At start, Initializing Pathfinding Search " + goal);
            }

            // In goal, ends
            /* if ((character.position - goal.position).sqrMagnitude <= 2.5f)         {
                 Debug.Log("Reached Goal");
                 return goal;
             }
             */
             // else, plan
             GlobalPath currentSolution;
             if (Astar.InProgress)
             {
                 Debug.Log("AStar In Progress");
                 var finished = Astar.Search(out currentSolution, true);
                
                  if (finished && currentSolution != null)
                 {
                    this.AStarSolution = currentSolution;
                     this.GlobalPath = StringPullingPathSmoothing.SmoothPath(character, currentSolution);
                     this.GlobalPath.CalculateLocalPathsFromPathPositions(character.position);
                    // gets first node
                    goal.position = this.GlobalPath.LocalPaths[0].EndPosition;
                     Debug.Log("Reached first pos " + goal);

                     return goal;
                 }
                else if(currentSolution != null && currentSolution.IsPartial)
                {
                    goal.position = currentSolution.PathPositions[0];

                    Debug.Log("Temp " + goal);
                    return goal;
                }
            }
             else
             {

                 if (GlobalPath.PathEnd(CurrentParam))
                 {
                     goal.position = GlobalPath.LocalPaths[GlobalPath.LocalPaths.Count - 1].GetPosition(1.0f);
                    Debug.Log("PathEnd " + goal);
                    return goal;
                }

                 CurrentParam = GlobalPath.GetParam(character.position, CurrentParam);

                 goal.position = GlobalPath.GetPosition(CurrentParam);
                 Debug.Log("Following path " + goal);
                 return goal;
             }
            Debug.Log("All failed " + goal);
            return new Goal();
         }
     }
 }
