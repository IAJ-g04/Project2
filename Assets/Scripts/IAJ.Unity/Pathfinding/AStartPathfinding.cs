using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class AStarPathfinding
    {
        public NavMeshPathGraph NavMeshGraph { get; protected set; }
        //how many nodes do we process on each call to the search method
        public uint NodesPerSearch { get; set; }

        public uint TotalProcessedNodes { get; protected set; }
        public int MaxOpenNodes { get; protected set; }
        public float TotalProcessingTime { get; protected set; }
        public bool InProgress { get; protected set; }

        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }

        public NavigationGraphNode GoalNode { get; protected set; }
        public NavigationGraphNode StartNode { get; protected set; }
        public Vector3 StartPosition { get; protected set; }
        public Vector3 GoalPosition { get; protected set; }

        //heuristic function
        public IHeuristic Heuristic { get; protected set; }

        public AStarPathfinding(NavMeshPathGraph graph, IOpenSet open, IClosedSet closed, IHeuristic heuristic)
        {
            this.NavMeshGraph = graph;
            this.Open = open;
            this.Closed = closed;
            this.NodesPerSearch = 75; //by default we process all nodes in a single request
            this.InProgress = false;
            this.Heuristic = heuristic;
        }

        public void InitializePathfindingSearch(Vector3 startPosition, Vector3 goalPosition)
        {
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.StartNode = this.Quantize(this.StartPosition);
            this.GoalNode = this.Quantize(this.GoalPosition);

            //if it is not possible to quantize the positions and find the corresponding nodes, then we cannot proceed
            if (this.StartNode == null || this.GoalNode == null) return;

            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)this.StartNode).AddConnectedPoly(this.StartPosition);
            ((NavMeshPoly)this.GoalNode).AddConnectedPoly(this.GoalPosition);

            this.InProgress = true;
            this.TotalProcessedNodes = 0;
            this.TotalProcessingTime = 0.0f;
            this.MaxOpenNodes = 0;

            var initialNode = new NodeRecord
            {
                gValue = 0,
                hValue = this.Heuristic.H(this.StartNode, this.GoalNode),
                node = this.StartNode
            };

            initialNode.fValue = AStarPathfinding.F(initialNode);

            this.Open.Initialize(); 
            this.Open.AddToOpen(initialNode);
            this.Closed.Initialize();
        }


        protected virtual void ProcessChildNode(NodeRecord bestNode, NavigationGraphEdge connectionEdge)
        {
            var childNodeRecord = GenerateChildNodeRecord(bestNode, connectionEdge);
            var oldChildNodeOpen = Open.SearchInOpen(childNodeRecord);
            var oldChildNodeClosed = Closed.SearchInClosed(childNodeRecord);

            if ((oldChildNodeClosed == null) && (oldChildNodeOpen == null))
            {
                Open.AddToOpen(childNodeRecord);
            }
            else if ((oldChildNodeOpen != null) && oldChildNodeOpen.fValue > childNodeRecord.fValue)
            {
                Open.Replace(oldChildNodeOpen, childNodeRecord);

            }
            else if ((oldChildNodeClosed != null) && oldChildNodeClosed.fValue > childNodeRecord.fValue)
            {
                Closed.RemoveFromClosed(oldChildNodeClosed);
                Open.AddToOpen(childNodeRecord);
            }

        }

        public bool Search(out GlobalPath solution, bool returnPartialSolution = false)
        {
            var time = UnityEngine.Time.realtimeSinceStartup;
            var nodesprocessed = 0;
            var maximumOpen = 0;
            


            //to determine the connections of the selected nodeRecord you need to look at the NavigationGraphNode' EdgeOut  list
            //something like this

            while (true)
            {
                if (Open.CountOpen() > maximumOpen) maximumOpen = Open.CountOpen();

                if (Open.CountOpen() == 0)
                {
                    solution = null;
                    InProgress = false;
                    TotalProcessedNodes += (uint)nodesprocessed;
                    TotalProcessingTime = UnityEngine.Time.realtimeSinceStartup - time;
                    MaxOpenNodes = maximumOpen;
                    return true;
                }

                NodeRecord bestNode = Open.GetBestAndRemove();

                if (nodesprocessed > NodesPerSearch)
                {
                    if (returnPartialSolution)
                        solution = CalculateSolution(bestNode, returnPartialSolution);
                    else
                        solution = null;
                    InProgress = true;
                    TotalProcessedNodes += (uint)nodesprocessed;
                    TotalProcessingTime = UnityEngine.Time.realtimeSinceStartup - time;
                    MaxOpenNodes = maximumOpen;
                    return false;
                }
                

                if (bestNode.node == GoalNode)
                {
                    
                    solution = CalculateSolution(bestNode, false);
                    InProgress = false;
                    TotalProcessedNodes += (uint)nodesprocessed;
                    TotalProcessingTime = UnityEngine.Time.realtimeSinceStartup - time;
                    MaxOpenNodes = maximumOpen;
                    return true;
                }

                Closed.AddToClosed(bestNode);
                nodesprocessed++;


                //
                //TODO put your code here  
                //or if you would like, you can change just these lines of code this in the original A* Pathfinding Base Class, 
                //create a ProcessChildNode method in the base class with the code from the previous A* algorithm.
                //if you do this, then you don't need to implement this search method method. Just delete this and don't forget to override the ProcessChildMethod if you do this
                var outConnections = bestNode.node.OutEdgeCount;
                for (int i = 0; i < outConnections; i++)
                {
                    this.ProcessChildNode(bestNode, bestNode.node.EdgeOut(i));
                }
                
            }
        }

        protected NavigationGraphNode Quantize(Vector3 position)
        {
            return this.NavMeshGraph.QuantizeToNode(position, 1.0f);
        }

        protected void CleanUp()
        {
            //I need to remove the connections created in the initialization process
            if (this.StartNode != null)
            {
                ((NavMeshPoly)this.StartNode).RemoveConnectedPoly();
            }

            if (this.GoalNode != null)
            {
                ((NavMeshPoly)this.GoalNode).RemoveConnectedPoly();    
            }
        }

        protected virtual NodeRecord GenerateChildNodeRecord(NodeRecord parent, NavigationGraphEdge connectionEdge)
        {
            var childNode = connectionEdge.ToNode;
            var childNodeRecord = new NodeRecord
            {
                node = childNode,
                parent = parent,
                gValue = parent.gValue + connectionEdge.Cost,
                hValue = this.Heuristic.H(childNode, this.GoalNode)
            };

            childNodeRecord.fValue = F(childNodeRecord);

            return childNodeRecord;
        }

        protected GlobalPath CalculateSolution(NodeRecord node, bool partial)
        {
            var path = new GlobalPath
            {
                IsPartial = partial
            };
            var currentNode = node;

            path.PathPositions.Add(this.GoalPosition);

            //I need to remove the first Node and the last Node because they correspond to the dummy first and last Polygons that were created by the initialization.
            //And for instance I don't want to be forced to go to the center of the initial polygon before starting to move towards my destination.

            //skip the last node, but only if the solution is not partial (if the solution is partial, the last node does not correspond to the dummy goal polygon)
            if (!partial && currentNode.parent != null)
            {
                currentNode = currentNode.parent;
            }
            
            while (currentNode.parent != null)
            {
                path.PathNodes.Add(currentNode.node); //we need to reverse the list because this operator add elements to the end of the list
                path.PathPositions.Add(currentNode.node.LocalPosition);

                if (currentNode.parent.parent == null) break; //this skips the first node
                currentNode = currentNode.parent;
            }

            path.PathNodes.Reverse();
            path.PathPositions.Reverse();
            return path;
        }

        public static float F(NodeRecord node)
        {
            return F(node.gValue,node.hValue);
        }

        public static float F(float g, float h)
        {
            return g + h;
        }

    }
}
