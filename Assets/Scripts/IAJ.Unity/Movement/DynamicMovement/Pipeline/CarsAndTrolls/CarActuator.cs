using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class CarActuator : Actuator    
    {
        public override string Name
        {
            get { return "CarActuator"; }
        }

        public override KinematicData Target { get; set; }

        public override LineSegmentPath GetPath()
        {
            LineSegmentPath segment;
            if (goal.hasPosition) {
                segment = new LineSegmentPath(this.Character.position, goal.position);               
            }
            else{
                segment = new LineSegmentPath(this.Character.position, this.Character.position);               
            }

            return segment;
        }

        public override MovementOutput GetMovement()
        {

           MovementOutput steering = new MovementOutput();

            // Percorrer cada canal
            if (goal.hasOrientation)
            {
                // usar o DynamicAlign...
                DynamicVelocityMatch da = new DynamicVelocityMatch()
                {
                    Character = this.Character,
                    Target = new KinematicData(),
                    MaxAcceleration = this.MaxAcceleration
                };
                da.Target.orientation = goal.orientation;
                steering.angular += da.GetMovement().angular;
            }

            if (goal.hasPosition)
            {
                // usar o DynamicSeek
                DynamicSeek ds = new DynamicSeek()
                {
                    Character = this.Character,
                    Target = new KinematicData(),
                    MaxAcceleration = this.MaxAcceleration
                };
                ds.Target.position = goal.position;
                steering.linear += ds.GetMovement().linear;
            }

            //  velocidades e possivelmente erros

            steering.linear.Normalize();
            steering.linear *= this.MaxAcceleration;
            steering.angular *= this.MaxAcceleration;            

            return steering;
        }
    }
}
