using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class SteeringPipeline : DynamicMovement
    {
        public override string Name
        {
            get { return "SteeringPipeline"; }
        }

        public override KinematicData Target { get; set; }

        public List<Targeter> Targeters { get; set; }
        public List<Decomposer> Decomposers { get; set; }
        public List<Constraint> Constraints { get; set; }
        public Actuator Actuator { get; set; }

        public GlobalPath GlobalPath { get; set; }
        private float MaxConstraintSteps = 5.0f;

        private DynamicMovement DeadlockMovement;

        public SteeringPipeline(KinematicData character)
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.DeadlockMovement = new DynamicWander
            {
                Character = this.Character,
                MaxAcceleration = 10.0f
            };
            this.Targeters = new List<Targeter>();
            this.Decomposers = new List<Decomposer>();
            this.Constraints = new List<Constraint>();
        }

        public override MovementOutput GetMovement()
        {
            Goal g = new Goal();
            foreach (Targeter t in Targeters)
            {
                g.UpdateChannels(t.GetGoal(Character));
            }
            foreach (Decomposer d in Decomposers)
            {
                g = d.Decompose(Character, g);
            }

            // bool ValidPath = false; TO DO esta parte ta uma bequita meh

            for (int i = 0; i <= MaxConstraintSteps; i++)
            {
                Actuator.goal = g;
                LineSegmentPath path = Actuator.GetPath();
                foreach (Constraint c in Constraints)
                {
                    if (c.WillViolate(path))
                    {
                        g = c.Suggest(path, Character, g);
                        continue;
                    }
                }
                return Actuator.GetMovement();
            }
            return DeadlockMovement.GetMovement();
        }
    }
}
