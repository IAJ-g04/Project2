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
    public class PathfindingDecomposer : Decomposer
    {
        public NodeArrayAStarPathFinding Astar { get; set; }
        public GlobalPath AStarSolution { get; set; }
        public float CurrentParam  {get;set;}
        

        public PathfindingDecomposer()
        {
            CurrentParam = 0.0f;
        }

        public override Goal Decompose (KinematicData character, Goal goal)
        {
            if((Astar == null)) { 
                Astar = new NodeArrayAStarPathFinding(Graph, Heuristic);
                Astar.InitializePathfindingSearch(character.position, goal.position);
                CurrentParam = 0.0f;
            }
            

            GlobalPath currentSolution;
             if (Astar.InProgress)
             {
                 var finished = Astar.Search(out currentSolution, true);
                
                  if (finished && currentSolution != null)
                 {
                    this.AStarSolution = currentSolution;
                     this.GlobalPath = StringPullingPathSmoothing.SmoothPath(character, currentSolution);
                     this.GlobalPath.CalculateLocalPathsFromPathPositions(character.position);
                    // gets first node
                    goal.position = this.GlobalPath.LocalPaths[0].EndPosition;
                     return goal;
                 }
               /* else if(currentSolution != null && currentSolution.IsPartial)
                {
                    goal.position = currentSolution.PathPositions[0];
                    return goal;
                }*/
            }
             else
             {
                if (GlobalPath.PathEnd(CurrentParam))
                 {
                     goal.position = GlobalPath.LocalPaths[GlobalPath.LocalPaths.Count - 1].GetPosition(1.0f);
                    return goal;
                }

                 CurrentParam = GlobalPath.GetParam(character.position, CurrentParam);


                 goal.position = GlobalPath.GetPosition(CurrentParam + 0.2f);
                 return goal;
             }
            return new Goal();
         }
     }
 }
