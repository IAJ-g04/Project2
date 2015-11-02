using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces
{
    interface Constraint
    {
        Boolean WillViolate(GlobalPath path);
        Goal Suggest(LineSegmentPath path, KinematicData character, Goal goal);
    }
}
