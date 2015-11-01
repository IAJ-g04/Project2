using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class Targeter
    {
        public float lookahead = 7.5f;
        public Goal GetGoal(KinematicData character)
        {
            Goal g = new Goal() { position = character.position + character.velocity * lookahead };
            g.hasPosition = true;

            return g;
        }
    }
}
