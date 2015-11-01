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
            Goal g = new Goal();
            foreach(Targeter t in Targeters)
            {
                g.UpdateChannels(t.GetGoal(Character));
            }
            foreach(Decomposer d in Decomposers)
            {
                g = d.Decompose(Character, g);
            }

           // bool ValidPath = false; TO DO esta parte ta uma bequita meh

               GlobalPath path = Actuator.GetPath(Character, g) as GlobalPath;
            LocalPath lpath = Actuator.GetPath(Character, g) as LocalPath;
            foreach (Constraint c in Constraints)
            {
                if (c.WillViolate(path))
                {
                    g = c.Suggest(lpath, Character, g);
                    continue;
                }
                return Actuator.GetMovement(Actuator.GetPath(Character, g), Character, g);
            }
            return DeadlockMovement.GetMovement();   
        }
    }
}
