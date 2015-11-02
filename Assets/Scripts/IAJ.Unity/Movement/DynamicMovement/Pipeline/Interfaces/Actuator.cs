﻿using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces
{
   public abstract class Actuator : DynamicMovement
    {
        public Goal goal { get; set; }

        public abstract LineSegmentPath GetPath();
        public override abstract MovementOutput GetMovement();
    }
}
