using System.Collections;
using System.Linq;
using System.Collections.Generic;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    //very simple (and unefficient) implementation of the open/closed sets
    public class Hashmap : IClosedSet
    {
        private Dictionary<NodeRecord,NodeRecord>  NodeRecords { get; set; }

        public Hashmap()
        {
            
            this.NodeRecords = new Dictionary<NodeRecord, NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear(); 
        }

        public int Count()
        {
            return this.NodeRecords.Count;
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord, nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression

            if (this.NodeRecords.ContainsKey(nodeRecord))
                return NodeRecords[nodeRecord];

            //  return this.NodeRecords.FirstOrDefault(n => n.Equals(nodeRecord));
            return null;
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords.Values;

        }
    }
}
