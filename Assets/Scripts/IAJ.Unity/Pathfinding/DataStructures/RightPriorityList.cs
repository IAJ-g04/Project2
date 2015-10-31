using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class RightPriorityList : IOpenSet, IComparer<NodeRecord>
    {
        private List<NodeRecord> Open { get; set; }

        public RightPriorityList()
        {
            this.Open = new List<NodeRecord>();    
        }
        public void Initialize()
        {
            this.Open.Clear();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            int index = this.Open.BinarySearch(nodeToBeReplaced, this);
            if (index >= 0)
            {
                this.Open[index] = nodeToReplace;
            }
        }

        public NodeRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.Open.Remove(best);
            return best;
        }

        public NodeRecord PeekBest()
        {
            return Open[Open.Count-1];
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            //a little help here, notice the difference between this method and the one for the LeftPriority list
            //...this one uses a different comparer with an explicit compare function (which you will have to define below)
            int index = this.Open.BinarySearch(nodeRecord,this);
            if (index < 0)
            {
                this.Open.Insert(~index, nodeRecord);
            }
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.Open.Remove(nodeRecord);
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            return this.Open.FirstOrDefault(n => n.Equals(nodeRecord));
        }

        public ICollection<NodeRecord> All()
        {
            return this.Open;
        }

        public int CountOpen()
        {
            return this.Open.Count;
        }

        public int Compare(NodeRecord x, NodeRecord y)
        {
            if (x.fValue < y.fValue)
                return 1;
            else if (x.fValue > y.fValue)
                return -1;
            else
                return 0;
        }
    }
}
