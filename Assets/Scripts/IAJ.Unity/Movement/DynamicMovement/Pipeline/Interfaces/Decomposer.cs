using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    public abstract class Decomposer
    {
        public NavMeshPathGraph Graph { get; set; }
        public GlobalPath GlobalPath { get; set; }
        public IHeuristic Heuristic { get; set; }
        public abstract Goal Decompose(KinematicData character, Goal goal);
    }
}
