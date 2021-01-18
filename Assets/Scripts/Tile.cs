using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector3 PositionCenter => positionCenter;
    public Vector3 MatrixIndex => matrixIndex;
    public float Priority =>  priority;
    public bool IsWall => isWall;
    public bool IsGrass => isGrass;
    public bool CloseToTheWall { get; set; }

    private Vector3 matrixIndex;  
    private Vector3 position;  
    private Vector3 size;

    private Vector3 positionCenter;
    private bool isWall;
    private bool isGrass;
    private float priority;
    private Vector3 tileTopCorner;
    private Vector3 tileDiagonalTileCorner;
    private Vector3 tileRightCorner;

    bool testDebug = true;
    public Tile(Vector3 indexPosition, Vector3 realPosition, Vector3 sizeParam) 
    {
        matrixIndex = indexPosition;
        position = realPosition;
        size = sizeParam;

        priority = 0;
        positionCenter = realPosition + sizeParam/2;

        tileTopCorner = position + new Vector3(size.x, 0, 0);
        tileDiagonalTileCorner = position + size;
        tileRightCorner = position + new Vector3(0, 0, size.z);
    }

    public void DrawTile(Color lineColor, int time = 0)
    {
        Debug.DrawLine(position, tileTopCorner, lineColor, time, false);
        Debug.DrawLine(position, tileRightCorner, lineColor, time, false);
        Debug.DrawLine(tileTopCorner, tileDiagonalTileCorner, lineColor, time, false);
        Debug.DrawLine(tileRightCorner, tileDiagonalTileCorner, lineColor, time, false);
    }

    public void CalculatePriority(TilePriorityCalculationInfo priorityCalculationData)
    {
        priority = 1;

        var distanceToPlayer = getDistanceToPlayer(priorityCalculationData.PlayerPosition);
        priority += (priorityCalculationData.DistanceToKeepFromPlayer/(priorityCalculationData.DistanceToKeepFromPlayer+distanceToPlayer)) * 2;

        if(distanceToPlayer > priorityCalculationData.DistanceToKeepFromPlayer)
        {
            priority += 1;
        }

        if(isWallOnTheWayToPlayer(priorityCalculationData.PlayerPosition, priorityCalculationData.LayerMaskWall)){
            priority += 3;
        }

        if(isGrass)
        {
            priority += 0.01f;
        }

        if(CloseToTheWall)
        {
            priority += 5; 
        }
    }

    public void UpdateTileTypeInfo(LayerMask layerMaskWall, LayerMask layerMaskGrass) 
    {        
        isWall = isObjectOnTheTile(layerMaskWall);
        isGrass = isObjectOnTheTile(layerMaskGrass);
    }

    private bool isObjectOnTheTile(LayerMask mask)
    {
        RaycastHit hit;
        var hitghPointToCastFromY = 100;
        return Physics.Raycast(positionCenter.OverrideY(hitghPointToCastFromY), Vector3.down, out hit, Mathf.Infinity, mask) ||
            Physics.Raycast(tileTopCorner.OverrideY(hitghPointToCastFromY), Vector3.down, out hit, Mathf.Infinity, mask) ||
            Physics.Raycast(tileRightCorner.OverrideY(hitghPointToCastFromY), Vector3.down, out hit, Mathf.Infinity, mask) ||
            Physics.Raycast(tileDiagonalTileCorner.OverrideY(hitghPointToCastFromY), Vector3.down, out hit, Mathf.Infinity, mask) ||
            Physics.Raycast(positionCenter.OverrideY(hitghPointToCastFromY), Vector3.down, out hit, Mathf.Infinity, mask);
    }

    private float getDistanceToPlayer(Vector3 playerPosition) 
    {
        return Vector3.Distance(position, playerPosition);
    }
    private bool isWallOnTheWayToPlayer(Vector3 playerPosition, LayerMask layerMaskWall) {
        var result = false;
        
        RaycastHit hit;
        var directionVector = playerPosition - positionCenter;
        var distanceToPlayer = getDistanceToPlayer(playerPosition);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(positionCenter, directionVector, out hit, distanceToPlayer, layerMaskWall))
        {
            // if(testDebug && matrixIndex.x == 15 && matrixIndex.z == 17)
            // {
            //     testDebug = false;
            //     //Debug.DrawRay(positionCenter, directionVector * hit.distance, Color.yellow, 100);
            //     //Debug.Log("Did Hit: " + hit.collider.name);
            // }
            
            result = true;
        }
        else
        {
            // if(testDebug)
            // {
            //     testDebug = false;
            //     Debug.DrawRay(positionCenter, directionVector * 100, Color.white, 100);
            //     Debug.Log("Did not Hit");
            // }

            result = false;
        }

        return result;
    }
}
