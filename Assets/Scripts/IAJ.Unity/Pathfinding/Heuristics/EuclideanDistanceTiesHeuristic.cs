using RAIN.Navigation.Graph;
using System;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistanceTiesHeuristic : IHeuristic
    {
        public float PValue { get; protected set; }

        public EuclideanDistanceTiesHeuristic(float p)
        {
            this.PValue = p;
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return (float)(Math.Sqrt(Math.Pow(goalNode.LocalPosition.x - node.LocalPosition.x, 2.0) +
                                Math.Pow(goalNode.LocalPosition.y - node.LocalPosition.y, 2.0)))*(1-this.PValue);
        }
    }
}
