using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class Targeter
    {
        public KinematicData Target { get; set; }
        public float LookAhead = 7.5f;
        public Goal GetGoal(KinematicData character)
        {
            Goal g = new Goal() { position = Target.position + Target.velocity * LookAhead };
            g.hasPosition = true;

            return g;
        }

        public override string ToString()
        {
            string returnvalue = string.Empty;
            returnvalue += "Targeter- ";

            returnvalue += "Position: " + Target.position;

            return returnvalue;
        }
    }
}
