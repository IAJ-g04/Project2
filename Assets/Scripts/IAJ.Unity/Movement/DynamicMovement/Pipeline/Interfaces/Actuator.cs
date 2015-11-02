using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces
{
   interface Actuator
    {
        LineSegmentPath GetPath();
        MovementOutput GetMovement();
    }
}
