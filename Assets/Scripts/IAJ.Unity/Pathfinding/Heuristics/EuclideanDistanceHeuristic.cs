using RAIN.Navigation.Graph;
using System;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistanceHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return (float)Math.Sqrt(Math.Pow(goalNode.LocalPosition.x - node.LocalPosition.x, 2.0) +
                                Math.Pow(goalNode.LocalPosition.y - node.LocalPosition.y, 2.0));
        }
    }
}
