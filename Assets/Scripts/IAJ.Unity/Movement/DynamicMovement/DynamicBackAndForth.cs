using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicBackAndForth : DynamicArrive
    {

        public const float NORTH = 0.0f;
        public const float EAST = 90.0f;
        public const float SOUTH = 180.0f;
        public const float WEST = 270.0f;

        public void CalculatePositions()
        {
            if (this.Character.orientation == NORTH)
            {
                this.BackPosition = new KinematicData();
                this.BackPosition.position = this.Character.position;
                this.BackPosition.position.z += MoveDistance;

                this.ForthPosition = new KinematicData();
                this.ForthPosition.position = this.Character.position;
                this.ForthPosition.position.z -= MoveDistance;
            }
            else if (this.Character.orientation == EAST)
            {
                this.BackPosition = new KinematicData();
                this.BackPosition.position = this.Character.position;
                this.BackPosition.position.x += MoveDistance;

                this.ForthPosition = new KinematicData();
                this.ForthPosition.position = this.Character.position;
                this.ForthPosition.position.x -= MoveDistance;
            }
            else if (this.Character.orientation == SOUTH)
            {
                this.BackPosition = new KinematicData();
                this.BackPosition.position = this.Character.position;
                this.BackPosition.position.z -= MoveDistance;

                this.ForthPosition = new KinematicData();
                this.ForthPosition.position = this.Character.position;
                this.ForthPosition.position.z += MoveDistance;
            }
            else
            {
                this.BackPosition = new KinematicData();
                this.BackPosition.position = this.Character.position;
                this.BackPosition.position.x -= MoveDistance;

                this.ForthPosition = new KinematicData();
                this.ForthPosition.position = this.Character.position;
                this.ForthPosition.position.x += MoveDistance;
            }

            this.Target = this.BackPosition;
           
        }

        public override string Name
        {
            get { return "BackAndForward"; }
        }
        
        public float MoveDistance { get; set; }
        public KinematicData BackPosition { get; set; }
        public KinematicData ForthPosition { get; set; }

        public override MovementOutput GetMovement()
        {

            if (this.Arrived)
            {
                if (Target.Equals(this.BackPosition))
                    this.Target = this.ForthPosition;
                else
                    this.Target = this.BackPosition;
                Arrived = false;
            }

            return base.GetMovement();
        }
    }
}