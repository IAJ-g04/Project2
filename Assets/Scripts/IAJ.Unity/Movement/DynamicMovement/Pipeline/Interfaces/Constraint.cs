using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    public abstract class Constraint
    {
        public abstract Boolean WillViolate(LineSegmentPath path);
        public abstract Goal Suggest(LineSegmentPath path, KinematicData character, Goal goal);
    }
}
