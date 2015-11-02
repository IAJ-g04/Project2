using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    public class CarActuator : Actuator    
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
                steering.angular = da.GetMovement().angular;
            }

            if (goal.hasPosition)
            {
                // usar o DynamicSeek
                DynamicArrive ds = new DynamicArrive()
                {
                    Character = this.Character,
                    Target = new KinematicData(),
                    MaxAcceleration = this.MaxAcceleration,
                    SlowRadius = 8.0f,
                    StopRadius = 4.0f
                };

     /*           if (ds.Target.orientation > goal.orientation)
                {
                    ds.Target.position += 0.1f;
                }
                else
                {
                    ds.Target.position -= 0.1f;
                }*/

                ds.Target.position = goal.position;
                steering.linear = ds.GetMovement().linear;
                
            }

            //  velocidades e possivelmente erros


            steering.linear.Normalize();
            steering.linear *= this.MaxAcceleration;
            steering.angular *= this.MaxAcceleration;
            steering.linear.y = 0.0f; // Failsafe
           
         

          //  Debug.Log("X->"+ Character.position.x + " Y->" + Character.position.y + " Z->" + Character.position.z);
          //  Debug.Log("Orientation->" + Character.orientation.ToString());
            return steering;
        }
    }
}
