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
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement.Pipeline.Interfaces;

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
                this.draw = true;
                //initialize the steering pipeline
                this.aStarPathFinding.InitializePathfindingSearch(this.character.KinematicData.position, this.endPosition);
                InitializeSteeringPipeline(this.character, new KinematicData(new StaticData(this.endPosition)));
            }
        }

        //call the pathfinding method if the user specified a new goal
        if (this.aStarPathFinding.InProgress)
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
        }
        

        this.character.Update();
        foreach (DynamicCharacter enemy in enemies)
        {
            enemy.Update();
        }
    }

    public void InitializeSteeringPipeline(DynamicCharacter orig, KinematicData dest)
    {
        Targeter MouseClickTargeter = new Targeter()
        {
            Target = dest
        };

        PathfindingDecomposer pathfindingDecomposer = new PathfindingDecomposer()
        {
            graph = this.navMesh,
            Heuristic = new EuclideanDistanceHeuristic()
         };


        //Default: Car behaviour
        Actuator actuator = new CarActuator()
        {
            MaxAcceleration = 30.0f,
            Character = orig.KinematicData
        };

        if (orig.GameObject.tag.Equals("Enemies")){
            actuator = new TrollActuator()
            {
                MaxAcceleration = 10.0f,
                Character = orig.KinematicData
            };
        }
        
        SteeringPipeline pipe = new SteeringPipeline(orig.KinematicData)
        {
            MaxAcceleration = 30.0f

        };
        pipe.Targeters.Add(MouseClickTargeter);
        pipe.Decomposers.Add(pathfindingDecomposer);
        pipe.Actuator = actuator;
    }

    public void OnGUI()
    {
        if (this.currentSolution != null)
        {
            var time = this.aStarPathFinding.TotalProcessingTime * 1000;
            float timePerNode;
            if (this.aStarPathFinding.TotalProcessedNodes > 0)
            {
                timePerNode = time / this.aStarPathFinding.TotalProcessedNodes;
            }
            else
            {
                timePerNode = 0;
            }
            var text = "Nodes Visited: " + this.aStarPathFinding.TotalProcessedNodes
                       + "\nMaximum Open Size: " + this.aStarPathFinding.MaxOpenNodes
                       + "\nProcessing time (ms): " + time
                       + "\nTime per Node (ms):" + timePerNode;
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(10, 10, 200, 100), text);
        }
    }

    public void OnDrawGizmos()
    {
        if (this.draw)
        {
            //draw the current Solution Path if any (for debug purposes)
            if (this.currentSolution != null && this.currentSmoothedSolution != null)
            {
                var previousPosition = this.startPosition;
                foreach (var pathPosition in this.currentSolution.PathPositions)
                {
                    Debug.DrawLine(previousPosition, pathPosition, Color.red);
                    previousPosition = pathPosition;
                }

                previousPosition = this.startPosition;
                foreach (var pathPosition in this.currentSmoothedSolution.PathPositions)
                {
                    Debug.DrawLine(previousPosition, pathPosition, Color.green);
                    previousPosition = pathPosition;
                }

            }

            //draw the nodes in Open and Closed Sets
            if (this.aStarPathFinding != null)
            {
                Gizmos.color = Color.cyan;

                if (this.aStarPathFinding.Open != null)
                {
                    foreach (var nodeRecord in this.aStarPathFinding.Open.All())
                    {
                        Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }

                Gizmos.color = Color.blue;

                if (this.aStarPathFinding.Closed != null)
                {
                    foreach (var nodeRecord in this.aStarPathFinding.Closed.All())
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
