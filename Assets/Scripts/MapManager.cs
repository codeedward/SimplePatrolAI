using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Map;
    public int TilesDensity;

    private Vector2 mapSize;
    private Vector2 tileSize;
    private Vector2 mapBottomLeftCorner;

    void Start()
    {
        if(TilesDensity <= 0 || Map == null || Player == null){
            print("Incorrect data in MapManager");
            enabled = false;
        }
        
        var mapRenderer = Map.GetComponent<MeshRenderer>();
        var sizeOfTheMap = mapRenderer.bounds.size;
        mapSize = new Vector2(sizeOfTheMap.x, sizeOfTheMap.z);
        tileSize = mapSize/TilesDensity;

        var mapLocation = Map.transform.position;
        mapBottomLeftCorner = new Vector2(mapLocation.x , mapLocation.z) - mapSize/2;

        // Debug.Log(mapSize);
        // Debug.Log(tileSize);
        // Debug.Log(mapBottomLeftCorner);   
    }

    void Update()
    {        
        VisualiseTiles();
    }

    public Vector2 GetTileWithPlayer(Vector3 playerPosition)
    {
        for (int x = 0; x < TilesDensity; x++)
        {
            var tileStartX = x * tileSize.x + mapBottomLeftCorner.x;

            if(playerPosition.x >= tileStartX && playerPosition.x < tileStartX + tileSize.x)
            {
                for (int y = 0; y < TilesDensity; y++)
                {
                     var tileStartY = y * tileSize.y + mapBottomLeftCorner.y;
                     if(playerPosition.z >= tileStartY && playerPosition.z < tileStartY + tileSize.y)
                     {
                         return new Vector2(x,y);
                     }
                }
            }
        }
        return Vector2.zero;
    }

    public void DrawPlayerTile()
    {
        var playerTile = GetTileWithPlayer(Player.transform.position);
        DrawTile(playerTile, Color.red, 2);   
    }

    public Vector3 GetCenterOfThePlayerTile()
    {
        var playerTile = GetTileWithPlayer(Player.transform.position);
        return GetCenterOfTheTile(playerTile);
    }

    private Vector3 GetCenterOfTheTile(Vector2 tile)
    {
        var tileStartX = tile.x * tileSize.x + mapBottomLeftCorner.x;
        var tileStartY = tile.y * tileSize.y + mapBottomLeftCorner.y;

        return new Vector3(tileStartX + tileSize.x/2, 0, tileStartY + tileSize.y/2);
    }

    private void VisualiseTiles()
    {
        for (int x = 0; x < TilesDensity; x++)
        {
            for (int y = 0; y < TilesDensity; y++)
            {
                DrawTile(new Vector2(x, y), Color.green);    
            }
        }
    }

    private void DrawTile(Vector2 tileCoordinates, Color lineColor, int time = 0){
        var initPosition = tileCoordinates * tileSize + mapBottomLeftCorner;
        var topCorner = initPosition + new Vector2(tileSize.x, 0);
        var diagonalTileCorner = initPosition + tileSize;
        var rightCorner = initPosition + new Vector2(0, tileSize.y);

        Debug.DrawLine(initPosition.GetMe3(), topCorner.GetMe3(), lineColor, time, false);
        Debug.DrawLine(initPosition.GetMe3(), rightCorner.GetMe3(), lineColor, time, false);
        Debug.DrawLine(topCorner.GetMe3(), diagonalTileCorner.GetMe3(), lineColor, time, false);
        Debug.DrawLine(rightCorner.GetMe3(), diagonalTileCorner.GetMe3(), lineColor, time, false);
    }
}
    

