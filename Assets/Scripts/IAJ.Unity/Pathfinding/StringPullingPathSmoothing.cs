using System;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.NavMesh;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public static class StringPullingPathSmoothing
    {
        /// <summary>
        /// Method used to smooth a received path, using a string pulling technique
        /// it returns a new path, where the path positions are selected in order to provide a smoother path
        /// </summary>
        /// <param name="data"></param>
        /// <param name="globalPath"></param>
        /// <returns></returns>
        public static GlobalPath SmoothPath(KinematicData data, GlobalPath globalPath)
        {

            if (globalPath.PathNodes.Count <= 2)
                return globalPath;

            var smoothedPath = new GlobalPath
            {
                IsPartial = globalPath.IsPartial
            };

            globalPath.PathNodes.Reverse();

            NavMeshEdge goalNode = globalPath.PathNodes[0] as NavMeshEdge;
            Vector3 pnext = goalNode.Position;

            globalPath.PathNodes.RemoveAt(0);
            smoothedPath.PathNodes.Add(goalNode);
            smoothedPath.PathPositions.Add(pnext);

            foreach (var node in globalPath.PathNodes)
            {
                NavMeshEdge edge = node as NavMeshEdge;
                if (edge != null)
                {
                    Vector3 point = MathHelper.ClosestPointInLineSegment2ToLineSegment1(data.position, pnext, edge.PointOne, edge.PointTwo, edge.PointOne);
                    pnext = point;
                    smoothedPath.PathNodes.Add(node);
                    smoothedPath.PathPositions.Add(point);
                }

            }

            smoothedPath.PathNodes.Reverse();
            smoothedPath.PathPositions.Reverse();
            return smoothedPath;
        }


    }
}
