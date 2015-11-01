using System;
using System.Collections.Generic;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; } 
        public bool IsPartial { get; set; }
        public List<LocalPath> LocalPaths { get; protected set; } 

        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LocalPath>();
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            Vector3 previousPosition = initialPosition;
            for (int i = 0; i < this.PathPositions.Count; i++)
            {

				if(!previousPosition.Equals(this.PathPositions[i]))
				{
					this.LocalPaths.Add(new LineSegmentPath(previousPosition,this.PathPositions[i]));
					previousPosition = this.PathPositions[i];
				}
            }
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
            int i = (int)Math.Floor(previousParam);

            LineSegmentPath path = LocalPaths[i] as LineSegmentPath;
           
            return i + path.GetParam(position, previousParam-i);

        }

        public override Vector3 GetPosition(float param)
        {
            int i = (int)Math.Floor(param);
            //HERE
            LineSegmentPath path = LocalPaths[i] as LineSegmentPath;

            return path.GetPosition(param - i);
        }

        public override bool PathEnd(float param)
        {
       
            if (param >= LocalPaths.Count - 1)
                return true;
            else
            {
                return false;
            }
        }
    }
}
