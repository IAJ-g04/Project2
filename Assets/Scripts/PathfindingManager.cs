using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using UnityEngine;
using RAIN.Navigation;
using RAIN.Navigation.NavMesh;
using RAIN.Navigation.Graph;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline;

public class PathfindingManager : MonoBehaviour
{

    //public fields to be set in Unity Editor
    public GameObject endDebugSphere;
    public Camera camera;
    public GameObject characterAvatar;
    public GameObject[] enemiesAvatar;

    //private fields for internal use only
    private Vector3 startPosition;
    private Vector3 endPosition;
    private NavMeshPathGraph navMesh;

    private AStarPathfinding aStarPathFinding;
    private GlobalPath currentSolution;
    private GlobalPath currentSmoothedSolution;
    private PathfindingDecomposer pathfindingDecomposer;

    private DynamicCharacter character;

    private List<DynamicCharacter> enemies;

    private bool draw;

    // Use this for initialization
    void Awake()
    {
        this.draw = false;
        this.navMesh = NavigationManager.Instance.NavMeshGraphs[0];
        this.character = new DynamicCharacter(this.characterAvatar);
        this.enemies = new List<DynamicCharacter>();
        for (int i = 0; i < enemiesAvatar.Length; i++)
        {
            DynamicCharacter enemy = new DynamicCharacter(this.enemiesAvatar[i]);
            DynamicBackAndForth movement = new DynamicBackAndForth()
            {
                Character = enemy.KinematicData,
                MaxAcceleration = 10.0f,
                StopRadius = 2.0f,
                SlowRadius = 5.0f,
                MoveDistance = 25.0f
            };
            movement.CalculatePositions();
            enemy.Movement = movement;
            this.enemies.Add(enemy);
        }
        
        this.aStarPathFinding = new NodeArrayAStarPathFinding(this.navMesh, new EuclideanDistanceHeuristic());
        this.aStarPathFinding.NodesPerSearch = 100;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position;
        NavigationGraphNode node;

        /* A*
        if (Input.GetMouseButtonDown(0))
        {
            //if there is a valid position
            if (this.MouseClickPosition(out position))
            {
                //we're setting the end point
                //this is just a small adjustment to better see the debug sphere
                this.endDebugSphere.transform.position = position + Vector3.up;
                this.endDebugSphere.SetActive(true);
                //this.currentClickNumber = 1;
                this.endPosition = position;
                this.draw = true;
                //initialize the search algorithm
                this.aStarPathFinding.InitializePathfindingSearch(this.character.KinematicData.position, this.endPosition);
            }
        }
        */

        if (Input.GetMouseButtonDown(0))
        {
            //if there is a valid position
            if (this.MouseClickPosition(out position))
            {
                //we're setting the end point
                //this is just a small adjustment to better see the debug sphere
                this.endDebugSphere.transform.position = position + Vector3.up;
                this.endDebugSphere.SetActive(true);
                //this.currentClickNumber = 1;
                this.endPosition = position;
                //initialize the steering pipeline
                // this.aStarPathFinding.InitializePathfindingSearch(this.character.KinematicData.position, this.endPosition);

                this.startPosition = this.character.KinematicData.position;
                this.character.Movement = InitializeSteeringPipeline(this.character, new KinematicData(new StaticData(this.endPosition)));
                this.draw = true;
            }
        }

        //call the pathfinding method if the user specified a new goal
       /* if (this.aStarPathFinding.InProgress)
        {
            var finished = this.aStarPathFinding.Search(out this.currentSolution);
            if (finished && this.currentSolution != null)
            {
                //lets smooth out the Path
                this.startPosition = this.character.KinematicData.position;
                this.currentSmoothedSolution = StringPullingPathSmoothing.SmoothPath(this.character.KinematicData, this.currentSolution);
                this.currentSmoothedSolution.CalculateLocalPathsFromPathPositions(this.character.KinematicData.position);
                this.character.Movement = new DynamicFollowPath(this.character.KinematicData, currentSmoothedSolution)
                {
                    MaxAcceleration = 30.0f
                };

            }
        }*/


        this.character.Update();
        foreach (DynamicCharacter enemy in enemies)
        {
            enemy.Update();
        }
    }

    public SteeringPipeline InitializeSteeringPipeline(DynamicCharacter orig, KinematicData dest)
    {
        //Pipeline
        SteeringPipeline pipe = new SteeringPipeline(orig.KinematicData)
        {
            MaxAcceleration = 15.0f

        };

        //Targeter
        Targeter MouseClickTargeter = new Targeter()
        {
            Target = dest
        };
        pipe.Targeters.Add(MouseClickTargeter);

        //Decomposer
        pathfindingDecomposer = new PathfindingDecomposer()
        {
            Graph = this.navMesh,
            Heuristic = new EuclideanDistanceHeuristic()
        };
        pipe.Decomposers.Add(pathfindingDecomposer);

        //Actuator - Default: Car behaviour
        Actuator actuator = new CarActuator()
        {
            MaxAcceleration = 15.0f,
            Character = orig.KinematicData
        };

        if (orig.GameObject.tag.Equals("Enemies"))
        {
            actuator = new TrollActuator()
            {
                MaxAcceleration = 10.0f,
                Character = orig.KinematicData
            };
        }
        pipe.Actuator = actuator;

        //Constraints
        foreach (DynamicCharacter troll in enemies)
        {
            TrollConstraint trollConstraint = new TrollConstraint()
            {
                Troll = troll,
                margin = 1.0f
        };
            pipe.Constraints.Add(trollConstraint);
        }

        return pipe;

    }

    public void OnGUI()
    {
        if (this.draw) { 
            if (this.pathfindingDecomposer.AStarSolution != null)
        {
            var time = this.pathfindingDecomposer.Astar.TotalProcessingTime * 1000;
            float timePerNode;
            if (this.pathfindingDecomposer.Astar.TotalProcessedNodes > 0)
            {
                timePerNode = time / this.pathfindingDecomposer.Astar.TotalProcessedNodes;
            }
            else
            {
                timePerNode = 0;
            }
            var text = "Nodes Visited: " + this.pathfindingDecomposer.Astar.TotalProcessedNodes
                       + "\nMaximum Open Size: " + this.pathfindingDecomposer.Astar.MaxOpenNodes
                       + "\nProcessing time (ms): " + time
                       + "\nTime per Node (ms):" + timePerNode;
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(10, 10, 200, 100), text);
        }
        }
    }

    public void OnDrawGizmos()
    {
        if (this.draw)
        {
            //draw the current Solution Path if any (for debug purposes)
            if (this.pathfindingDecomposer.AStarSolution != null && this.pathfindingDecomposer.GlobalPath != null)
            {
                var previousPosition = this.startPosition;
                foreach (var pathPosition in this.pathfindingDecomposer.AStarSolution.PathPositions)
                {
                    Debug.DrawLine(previousPosition, pathPosition, Color.red);
                    previousPosition = pathPosition;
                }

                previousPosition = this.startPosition;
                foreach (var pathPosition in this.pathfindingDecomposer.GlobalPath.PathPositions)
                {
                    Debug.DrawLine(previousPosition, pathPosition, Color.green);
                    previousPosition = pathPosition;
                }

            }

            //draw the nodes in Open and Closed Sets
            if (this.pathfindingDecomposer.Astar != null)
            {
                Gizmos.color = Color.cyan;

                if (this.pathfindingDecomposer.Astar.Open != null)
                {
                    foreach (var nodeRecord in this.pathfindingDecomposer.Astar.Open.All())
                    {
                        Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }

                Gizmos.color = Color.blue;

                if (this.pathfindingDecomposer.Astar.Closed != null)
                {
                    foreach (var nodeRecord in this.pathfindingDecomposer.Astar.Closed.All())
                    {
                        Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }
            }

            Gizmos.color = Color.yellow;
            //draw the target for the follow path movement
            if (this.character.Movement != null)
            {
                Gizmos.DrawSphere(this.character.Movement.Target.position, 1.0f);
            }
        }
    }

    private bool MouseClickPosition(out Vector3 position)
    {
        RaycastHit hit;

        var ray = this.camera.ScreenPointToRay(Input.mousePosition);
        //test intersection with objects in the scene
        if (Physics.Raycast(ray, out hit))
        {
            //if there is a collision, we will get the collision point
            position = hit.point;
            return true;
        }

        position = Vector3.zero;
        //if not the point is not valid
        return false;
    }
}
