using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;
using RAIN.Navigation.NavMesh;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class MapConstraint : Constraint
    {
        public float margin { set; get; }
        public const float WallRadius = 0.2f;
        //  public Vector3 center { set; get; }
        public int probInd { set; get; }
        public NavMeshPathGraph navMeshP { set; get; }
        public DynamicCharacter chars { set; get; }

        public override Boolean WillViolate(LineSegmentPath path)
        {
            /* List<NavigationGraphNode> nodes = (List<NavigationGraphNode>)Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), path, "_pathNodes");

             foreach (NavigationGraphNode nvn in nodes) {
                 if (nvn.Position.Equals((chars.KinematicData.position + chars.KinematicData.velocity * 0.8f)))
                 {
                     return false;
                 }
             }
             return true;*/
             if(navMeshP.IsPointOnGraph((chars.KinematicData.position + chars.KinematicData.velocity * 0.5f)))
             {
                 return false;
             }
             else
             {
                 return true;
             }
        }


        public override Goal Suggest(LineSegmentPath path, KinematicData character, Goal goal)
        {
            // procurar ponto do segmento mais próximo ao centro da esfera
            Vector3 closest = path.GetPosition(MathHelper.closestParamInLineSegmentToPoint(path.StartPosition, path.EndPosition, chars.KinematicData.position));
            // Check if we pass through the center point

            Vector3 newPt;
            float i = 1.0f;
            while (i < 25.0f)
            {
                for(int a = 0; a < 360; a++)
                {
                    float nx = (float) (closest.sqrMagnitude * i * Math.Cos(a));
                    float ny = (float) (closest.sqrMagnitude * i * Math.Sin(a));
                    newPt = new Vector3(nx, closest.y,ny);
                    if (navMeshP.IsPointOnGraph(newPt))
                    {
                        goal.position = newPt;
                        return goal;
                    }
                }
                i = i + 0.25f;
            }
            return goal;
            // Set up the goal and return
        }
    }
}
