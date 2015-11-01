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

        public float PredictTime { get; set; }

        public DynamicFollowPath(KinematicData character, GlobalPath path)
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.globalPath = path;
            this.CurrentParam = 0.0f;
            this.PathOffset = 0.2f;
            this.PredictTime = 0.05f;
        }

        public override MovementOutput GetMovement()
        {
            if (PathEnd()) {
                this.Target.position = this.globalPath.LocalPaths[this.globalPath.LocalPaths.Count - 1].GetPosition(1.0f);
                return base.GetMovement();
            }

            Vector3 futurePos = Character.position + Character.velocity * this.PredictTime;
            CurrentParam = globalPath.GetParam(futurePos, CurrentParam);

            this.Target.position = globalPath.GetPosition(CurrentParam + PathOffset);
            return base.GetMovement();
        }


        public bool PathEnd ()
        {
            return globalPath.PathEnd(CurrentParam);
        }

    }
}
