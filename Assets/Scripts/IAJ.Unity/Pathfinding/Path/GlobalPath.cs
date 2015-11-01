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

           /* if (this.PathEnd(previousParam))
                return LocalPaths.Count;
                */
            int i = (int)Math.Floor(previousParam);

            LineSegmentPath path = LocalPaths[i] as LineSegmentPath;

            /*if (path.PathEnd(previousParam - i))
            {
                i++;
                path = LocalPaths[i] as LineSegmentPath;
               // Debug.Log(i);
            }*/

            Debug.Log(i + path.GetParam(position, previousParam));
            return i + path.GetParam(position, previousParam);

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
            int i = (int)Math.Floor(param);

            if (i <= LocalPaths.Count - 2)
                return false;
            else
            {
                param = param - i;
                if (param <= PATHEND)
                    return false;
                else
                    return true;
            }
        }
    }
}
