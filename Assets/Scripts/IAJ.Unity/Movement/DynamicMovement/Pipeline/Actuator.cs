using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class Actuator
    {
        float maxAcceleration { get; set; }
        float maxRotation { get; set; }

    public Path GetPath(KinematicData character, Goal goal)
        {
            LineSegmentPath segment;
            if (goal.hasPosition) {
                segment = new LineSegmentPath(character.position, goal.position);               
            }
            else{
                segment = new LineSegmentPath(character.position, character.position);               
            }

            return segment;
        }

        public MovementOutput GetMovement(Path path, KinematicData character, Goal goal)
        {

           MovementOutput steering = new MovementOutput();

            // Percorrer cada canal
            if (goal.hasOrientation)
            {
                // usar o DynamicAlign...
                DynamicVelocityMatch da = new DynamicVelocityMatch();
                da.Target.orientation = goal.orientation;
                da.Character.orientation = character.orientation;
                steering.angular += da.GetMovement().angular;
            }

            if (goal.hasPosition)
            {
                // usar o DynamicSeek
                DynamicSeek ds = new DynamicSeek();
                ds.Target.position = goal.position;
                ds.Character.position = character.position;
                steering.linear += ds.GetMovement().linear;
            }

            //  velocidades
            steering.linear = min(steering.linear, maxAcceleration)
        steering.angular = min(steering.angular, maxRotation)

        return steering
        }
    }
}
