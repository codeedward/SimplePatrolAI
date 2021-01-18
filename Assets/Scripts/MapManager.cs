using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Map;
    public int TilesDensity;
    public bool VisualiseTilesOn;
    public LayerMask LayerMaskWall;
    public LayerMask LayerMaskGrass;
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

    // public void DrawPlayerTile()
    // {
    //     var playerTile = getTileForPosition(Player.transform.position);
    //     playerTile.DrawTile(Color.red, 2);   
    // }

    public Vector3 GetCenterOfThePlayerTile()
    {
        var playerTile = getTileForPosition(Player.transform.position);
        return playerTile.PositionCenter;
    }

    public Vector3 GetTileSize()
    {
        return tileSize;
    }

    public bool CanSeeThePlayer(Transform firstObjectTransform, float angleOfView, float distanceOfView)
    {
        var angleToPlayer = getAngleToPlayer(firstObjectTransform, Player.transform);
        var distanceToPlayer = getDistanceToPlayer(firstObjectTransform.position);
        return angleToPlayer <= angleOfView/2 && distanceToPlayer <= distanceOfView;
    }

    public Tile GetSuggestedLocation(Vector3 alienPosition, float rangeOfMove, float distanceToKeepFromPlayer)
    {        
        var rangeOfMoveInTiles = rangeOfMove/tileSize.x;

        if(rangeOfMoveInTiles < 1) print("Range of move is much too short!");

        var alienTile = getTileForPosition(alienPosition);
        var tilesInRange = getTilesInRange(alienTile, rangeOfMoveInTiles, distanceToKeepFromPlayer);

        foreach (var item in tilesInRange)
        {
            item.DrawTile(Color.magenta, 2);
        }

        return getTileWithHighestPriority(tilesInRange);
    }

    private List<Tile> getTilesInRange(Tile tile, float rangeOfMoveInTiles, float distanceToKeepFromPlayer)
    {
        var result = new List<Tile>();

        var firstTileInTheMatrixRangeIndex = tile.MatrixIndex - new Vector3(rangeOfMoveInTiles, 0, rangeOfMoveInTiles);
        var lastTileInTheMatrixRangeIndex = tile.MatrixIndex + new Vector3(rangeOfMoveInTiles, 0, rangeOfMoveInTiles);
        var minForLoop = new Vector3(
            firstTileInTheMatrixRangeIndex.x > 0f ? firstTileInTheMatrixRangeIndex.x : 0f,
            0,
            firstTileInTheMatrixRangeIndex.z > 0f ? firstTileInTheMatrixRangeIndex.z : 0f
        );
        var maxForLoop = new Vector3(
            lastTileInTheMatrixRangeIndex.x < matrixOfTiles.GetLength(0) ? lastTileInTheMatrixRangeIndex.x : matrixOfTiles.GetLength(0) - 1,
            0,
            lastTileInTheMatrixRangeIndex.z < matrixOfTiles.GetLength(1) ? lastTileInTheMatrixRangeIndex.z : matrixOfTiles.GetLength(1) - 1
        );
        
        var priorityCalculationData = new TilePriorityCalculationInfo(){
            PlayerPosition = Player.transform.position,
            DistanceToKeepFromPlayer = distanceToKeepFromPlayer,
            LayerMaskWall = LayerMaskWall,
            LayerMaskGrass = LayerMaskGrass
        };

        for(int x = (int)minForLoop.x; x < (int)maxForLoop.x; x++)
        {
           for (int z = (int)minForLoop.z; z < (int)maxForLoop.z; z++)
           {
               var currentTile = matrixOfTiles[x,z];
               if(!currentTile.IsWall)
               {
                    currentTile.CalculatePriority(priorityCalculationData);
                    result.Add(currentTile);
               }
           } 
        }

        return result;
    }

    private Tile getTileWithHighestPriority(List<Tile> tiles)
    {   
        var tilesWithHighestPriority = tiles.Where(x=> (int)x.Priority == tiles.Max(y => (int)y.Priority)).ToList();
        tilesWithHighestPriority.ForEach(x=>x.DrawTile(Color.yellow, 3));
        return tilesWithHighestPriority.Where(x=> x.Priority == tilesWithHighestPriority.Max(y => y.Priority)).FirstOrDefault();
    }

    private Tile getTileForPosition(Vector3 position)
    {
        for (int x = 0; x < TilesDensity; x++)
        {
            var tileStartX = x * tileSize.x + mapBottomLeftCornerPosition.x;

            if(position.x >= tileStartX && position.x < tileStartX + tileSize.x)
            {
                for (int z = 0; z < TilesDensity; z++)
                {
                     var tileStartZ = z * tileSize.z + mapBottomLeftCornerPosition.z;
                     if(position.z >= tileStartZ && position.z < tileStartZ + tileSize.z)
                     {
                         return matrixOfTiles[x, z];
                     }
                }
            }
        }
        return null;
    }

    private float getDotValue(Transform firstObject, Transform secondObject, Vector3 referenceVector)
    {
        if(firstObject == null || secondObject == null || referenceVector == null) 
        {
            print("Wrong parameters in GetDotValue method");
            return Mathf.Infinity;
        }

        var forward = firstObject.TransformDirection(referenceVector);
        var toPlayer = secondObject.position - firstObject.position;

        return Vector3.Dot(forward.normalized, toPlayer.normalized);
    }

    private float getAngleToPlayer(Transform firstObjectTransform, Transform secondObjectTransform)
    {
        return Mathf.Acos(getDotValue(firstObjectTransform, secondObjectTransform, Vector3.forward)) * Mathf.Rad2Deg;
    }

    private float getDistanceToPlayer(Vector3 position)
    {
       return Vector3.Distance(position, Player.transform.position);
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
                //tile.UpdateTileTypeInfo(LayerMaskWall, LayerMaskGrass);
                matrixOfTiles[x,z] = tile;
            }
        }

        for (int x = 0; x < TilesDensity; x++)
        {
            for (int z = 0; z < TilesDensity; z++)
            {       
                var currentTile = matrixOfTiles[x,z];     
                currentTile.UpdateTileTypeInfo(LayerMaskWall, LayerMaskGrass);
                if(currentTile.IsWall)
                {
                    updateNeighboursAboutTheWall(currentTile);
                }
            }
        }

        //For debugging only
        for (int x = 0; x < TilesDensity; x++)
        {
            for (int z = 0; z < TilesDensity; z++)
            {       
                var currentTile = matrixOfTiles[x,z];                     
                if(currentTile.CloseToTheWall)
                {
                    currentTile.DrawTile(Color.black, 10000);
                }
            }
        }
    }

    private void updateNeighboursAboutTheWall(Tile tile)
    {
        var currentX = (int)tile.MatrixIndex.x;
        var currentZ = (int)tile.MatrixIndex.z;
        var left = currentX - 1;
        var right = currentX + 1;
        var top = currentZ + 1;
        var down = currentZ - 1;

        if(left > 0)
        {
            matrixOfTiles[left, currentZ].CloseToTheWall = true;
            if(top < matrixOfTiles.GetLength(1)) matrixOfTiles[left, top].CloseToTheWall = true;
            if(down >= 0) matrixOfTiles[left, down].CloseToTheWall = true;
        }

        if(right < matrixOfTiles.GetLength(0))
        {
            matrixOfTiles[right, currentZ].CloseToTheWall = true;
            if(top < matrixOfTiles.GetLength(1)) matrixOfTiles[right, top].CloseToTheWall = true;
            if(down >= 0) matrixOfTiles[right, down].CloseToTheWall = true;
        }

        if(top < matrixOfTiles.GetLength(1)) matrixOfTiles[currentX, top].CloseToTheWall = true;
        if(down >= 0) matrixOfTiles[currentX, down].CloseToTheWall = true;
    }
}
    

