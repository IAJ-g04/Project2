using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    public class Goal : KinematicData
    {

        public bool hasPosition { get; set; }
     
        public bool hasOrientation { get; set; }

        public bool hasVelocity { get; set; }

        public bool hasRotation { get; set; }

        public Goal()
        {
            hasPosition = false;
            hasOrientation = false;
            hasVelocity = false;
            hasRotation = false;
        }

        public void UpdateChannels(Goal o){
            if (o.hasPosition)
            {
                position = o.position;
                hasPosition = true;
            }
            if (o.hasOrientation)
            {
                orientation = o.orientation;
                hasOrientation = true;
            }
            if (o.hasVelocity) { 
                velocity = o.velocity;
                hasVelocity = true;
            }
            if (o.hasRotation) { 
                rotation = o.rotation;
                hasRotation = true;
            }
        }

        public override string ToString()
        {
            string returnvalue = string.Empty;
            returnvalue += "Goal- ";

            if (hasPosition)
                returnvalue += "Position: " + position + "|";
            if (hasOrientation)
                returnvalue += "Orientation: " + orientation + "|";
            if (hasVelocity)
                returnvalue += "Velocity: " + velocity + "|";
            if (hasRotation)
                returnvalue += "Rotation: " + rotation + "|";

            return returnvalue;
        }

    }
}
