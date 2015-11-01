using System;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicSeek
    {
        public GlobalPath globalPath { get; set; }
        
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        public DynamicFollowPath(KinematicData character, GlobalPath path)
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.globalPath = path;
            this.CurrentParam = 0.0f;
            this.PathOffset = 0.005f;
        }

        public override MovementOutput GetMovement()
        {
            if (PathEnd())
                return new MovementOutput();

            CurrentParam = globalPath.GetParam(Character.position, CurrentParam);
            Debug.Log(CurrentParam);
            this.Target.position = globalPath.GetPosition(CurrentParam + PathOffset);

            return base.GetMovement();
        }


        public bool PathEnd ()
        {
            return globalPath.PathEnd(CurrentParam);
        }

    }
}
