using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces
{
    interface Decomposer
    {
        Goal Decompose(KinematicData character, Goal goal);
    }
}
