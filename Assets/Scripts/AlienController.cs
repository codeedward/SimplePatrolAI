using System.Collections;
using System.Collections.Generic;
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
    private NavMeshAgent agent;
    private Vector3? targetPosition;
    private bool destinationReached;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(MapManager == null || agent == null)
        {
            print("AlienController wrongly configured.");
            enabled = false;
        }
    }

    void Update()
    {
        WatchForPlayer();

        if(agent.remainingDistance > 0 && agent.remainingDistance != Mathf.Infinity && agent.remainingDistance <= agent.stoppingDistance && !destinationReached)
        {
            //Debug.Log("destination reached, distance: " + agent.remainingDistance);
            destinationReached = true;
            targetPosition = null;   
            LookAround();
        }   

        //Debug.Log(agent.remainingDistance);
        if (Input.GetKeyDown(KeyCode.E)){
            targetPosition = MapManager.GetCenterOfThePlayerTile();
            if(targetPosition.HasValue){
                destinationReached = false;
                agent.destination = targetPosition.Value;
                //Debug.Log("Distance just after change:" + agent.remainingDistance);
                MapManager.DrawPlayerTile();
            }
        }
    }

    void LookAround()
    {
        var startingPosition = CentralPointOfView.position;
        var inFrontPosition = CentralPointOfView.position + transform.forward*5;
        var towardsGround = new Vector3(inFrontPosition.x, -1, inFrontPosition.z);
        RaycastHit hit;

        Debug.DrawLine(startingPosition, towardsGround, Color.blue, 10, false);

        if(Physics.Raycast(startingPosition, towardsGround, out hit, Mathf.Infinity, GroundLayerMask.value))
        {
            Debug.Log(hit.collider.tag);
            if(hit.collider.tag == "Ground")
            {
                Debug.Log(hit.collider.name);
                
                // Gizmos.color = Color.magenta;
                // Gizmos.DrawSphere(hit.collider.gameObject.transform.position, 0.3f);
            }
        }
    }

    float GetDotValue(Vector3 baseVector, Vector3 relativePosition)
    {
        if(baseVector == null || relativePosition == null) 
        {
            print("Wrong parameters in GetDotValue method");
            return Mathf.Infinity;
        }

        var forward = transform.TransformDirection(baseVector);
        var toPlayer = relativePosition - transform.position;

        return Vector3.Dot(forward.normalized, toPlayer.normalized);
    }

    float GetAngleToPlayer()
    {
        return Mathf.Acos(GetDotValue(Vector3.forward, MapManager.Player.transform.position)) * Mathf.Rad2Deg;
    }

    float GetDistanceToPlayer()
    {
       return Vector3.Distance(transform.position, MapManager.Player.transform.position);
    }

    void WatchForPlayer()
    {
        var angleToPlayer = GetAngleToPlayer();
        var distanceToPlayer = GetDistanceToPlayer();
        var canSeeThePlayer = angleToPlayer <= AngleOfView/2 && distanceToPlayer <= DistanceOfView;
        CanSeePlayerIndicator.SetActive(canSeeThePlayer);
    }

    void OnDrawGizmos()
    {
        if(targetPosition.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition.Value, 0.5f);
        }
    }
}
