using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Vector3 matrixIndex;  
    private Vector3 position;  
    private Vector3 size;

    private Vector3 positionCenter;
    private bool isObstacle;
    private int priority;

    public Vector3 PositionCenter => positionCenter;

    public Tile(Vector3 indexPosition, Vector3 realPosition, Vector3 sizeParam) 
    {
        matrixIndex = indexPosition;
        position = realPosition;
        size = sizeParam;

        priority = 0;
        positionCenter = realPosition + sizeParam/2;
    }

    public void DrawTile(Color lineColor, int time = 0){
        var topCorner = position + new Vector3(size.x, 0, 0);
        var diagonalTileCorner = position + size;
        var rightCorner = position + new Vector3(0, 0, size.z);

        Debug.DrawLine(position, topCorner, lineColor, time, false);
        Debug.DrawLine(position, rightCorner, lineColor, time, false);
        Debug.DrawLine(topCorner, diagonalTileCorner, lineColor, time, false);
        Debug.DrawLine(rightCorner, diagonalTileCorner, lineColor, time, false);
    }
}
