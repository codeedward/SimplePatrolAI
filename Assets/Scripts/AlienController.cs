using UnityEngine;
using UnityEngine.AI;

public class AlienController : MonoBehaviour
{
    public MapManager MapManager;
    public Transform CentralPointOfView;
    public LayerMask GroundLayerMask;
    public GameObject CanSeePlayerIndicator;
    public float AngleOfView;
    public float DistanceOfView;
    public float DistanceForOneMove;
    public float DistanceToKeepFromPlayer;

    private NavMeshAgent agent;
    private Vector3? targetPosition;
    private bool isDestinationReached;
    private LineRenderer pathVisualisation;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathVisualisation = GetComponent<LineRenderer>();

        if(MapManager == null || agent == null || pathVisualisation == null)
        {
            print("AlienController wrongly configured.");
            enabled = false;
        }
    }

    void OnDrawGizmos()
    {
        if(targetPosition.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition.Value, 0.5f);
        }
    }

    void Update()
    {
        updatePathVisualisation();
        watchForPlayer();

        if (Input.GetKeyDown(KeyCode.E)){
            targetPosition = MapManager.GetCenterOfThePlayerTile();
            if(targetPosition.HasValue) {
                isDestinationReached = false;
                agent.destination = targetPosition.Value;
                //Debug.Log("Distance just after change:" + agent.remainingDistance);
                //MapManager.DrawPlayerTile();
            }
        }

        isDestinationReached = (agent.remainingDistance > 0 && agent.remainingDistance != Mathf.Infinity && agent.remainingDistance <= agent.stoppingDistance) || 
                                !agent.hasPath;

        if(isDestinationReached){
            var newLocation = MapManager.GetSuggestedLocation(transform.position, DistanceForOneMove, DistanceToKeepFromPlayer);
            agent.destination = newLocation.PositionCenter;
            //newLocation.DrawTile(Color.red, 2);   
            targetPosition = newLocation.PositionCenter;
        }
    }

    // void lookAround()
    // {
    //     var startingPosition = CentralPointOfView.position;
    //     var inFrontPosition = CentralPointOfView.position + transform.forward*5;
    //     var towardsGround = new Vector3(inFrontPosition.x, -1, inFrontPosition.z);
    //     RaycastHit hit;

    //     Debug.DrawLine(startingPosition, towardsGround, Color.blue, 10, false);
        
    //     if(Physics.Raycast(startingPosition, towardsGround, out hit, Mathf.Infinity, GroundLayerMask.value))
    //     {
    //         Debug.Log(hit.collider.tag);
    //         if(hit.collider.tag == "Ground")
    //         {
    //             Debug.Log(hit.collider.name);
                
    //             // Gizmos.color = Color.magenta;
    //             // Gizmos.DrawSphere(hit.collider.gameObject.transform.position, 0.3f);
    //         }
    //     }
    // }

    void watchForPlayer()
    {
        var canSeeThePlayer = MapManager.CanSeeThePlayer(transform, AngleOfView, DistanceOfView);
        CanSeePlayerIndicator.SetActive(canSeeThePlayer);
    }

    void updatePathVisualisation()
    {
        if(agent.hasPath) 
        {
            pathVisualisation.positionCount = agent.path.corners.Length;
            pathVisualisation.SetPositions(agent.path.corners);
            pathVisualisation.enabled = true;
        }
        else
        {
            pathVisualisation.enabled = false;
        }
    }
}
