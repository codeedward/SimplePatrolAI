using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Map;
    public int TilesDensity;
    public bool VisualiseTilesOn;
    private Vector3 mapSize;
    private Vector3 tileSize;
    private Vector3 mapBottomLeftCornerPosition;
    private Tile[,] matrixOfTiles;

    void Awake()
    {
        if(TilesDensity <= 0 || Map == null || Player == null){
            print("Incorrect data in MapManager");
            enabled = false;
        }
        
        var mapRenderer = Map.GetComponent<MeshRenderer>();
        var sizeOfTheMap = mapRenderer.bounds.size;
        mapSize = new Vector3(sizeOfTheMap.x, 0, sizeOfTheMap.z);
        tileSize = mapSize/TilesDensity;

        var mapLocation = Map.transform.position;
        mapBottomLeftCornerPosition = mapLocation - mapSize/2;

        // Debug.Log(mapSize);
        // Debug.Log(tileSize);
        // Debug.Log(mapBottomLeftCorner);   
        initMatrix();
    }

    void Update()
    {      
        if(VisualiseTilesOn) visualiseTiles();
    }

    public Vector3 GetTileWithPlayer(Vector3 playerPosition)
    {
        for (int x = 0; x < TilesDensity; x++)
        {
            var tileStartX = x * tileSize.x + mapBottomLeftCornerPosition.x;

            if(playerPosition.x >= tileStartX && playerPosition.x < tileStartX + tileSize.x)
            {
                for (int z = 0; z < TilesDensity; z++)
                {
                     var tileStartZ = z * tileSize.z + mapBottomLeftCornerPosition.z;
                     if(playerPosition.z >= tileStartZ && playerPosition.z < tileStartZ + tileSize.z)
                     {
                         return new Vector3(x, 0, z);
                     }
                }
            }
        }
        return Vector2.zero;
    }

    public void DrawPlayerTile()
    {
        var playerTile = GetTileWithPlayer(Player.transform.position);
        matrixOfTiles[(int)playerTile.x, (int)playerTile.z].DrawTile(Color.red, 2);   
    }

    public Vector3 GetCenterOfThePlayerTile()
    {
        var playerTile = GetTileWithPlayer(Player.transform.position);
        return matrixOfTiles[(int)playerTile.x, (int)playerTile.z].PositionCenter;
    }

    public Vector3 GetTileSize()
    {
        return tileSize;
    }

    private void visualiseTiles()
    {
        for (int x = 0; x < TilesDensity; x++)
        {
            for (int z = 0; z < TilesDensity; z++)
            {
                matrixOfTiles[x,z].DrawTile(Color.green);
            }
        }
    }

    private void initMatrix()
    {
        matrixOfTiles = new Tile[TilesDensity, TilesDensity];
        for (int x = 0; x < TilesDensity; x++)
        {
            for (int z = 0; z < TilesDensity; z++)
            {
                var matrixIndex = new Vector3(x, 0, z);
                var tilePosition = Vector3.Scale(tileSize, matrixIndex) +  mapBottomLeftCornerPosition;
                var tile = new Tile(matrixIndex, tilePosition, tileSize);                
                matrixOfTiles[x,z] = tile;
            }
        }
    }
}
    

