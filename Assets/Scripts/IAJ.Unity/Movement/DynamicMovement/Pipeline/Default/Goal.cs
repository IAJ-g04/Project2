using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class Goal : KinematicData
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
                position = o.position;
            if (o.hasOrientation)
                orientation = o.orientation;
            if (o.hasVelocity)
                velocity = o.velocity;
            if (o.hasRotation)
                rotation = o.rotation;
        }

    }
}
