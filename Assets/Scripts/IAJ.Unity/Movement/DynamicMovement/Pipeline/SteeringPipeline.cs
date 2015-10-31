using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline
{
    class SteeringPipeline : DynamicMovement
{
    public override string Name
    {
        get { return "SteeringPipeline"; }
    }

    public override KinematicData Target { get; set; }

    public List<Targeter> Targeters { get; private set; }
    public List<Decomposer> Decomposers { get; private set; }
    public List<Constraint> Constraints { get; private set; }
    public Actuator Actuator { get; private set; }

    private float MaxConstraintSteps = 5.0f;

    private DynamicMovement DeadlockMovement;

    public SteeringPipeline(KinematicData character)
        {
            this.Target = new KinematicData();
            this.Character = character;
        }

        public override MovementOutput GetMovement()
        {
            throw new NotImplementedException();
        }
    }
}
