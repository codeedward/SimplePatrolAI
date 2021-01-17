using UnityEngine;

public class TilePriorityCalculationInfo
{
    public Vector3 PlayerPosition { get; set; }
    public float DistanceToKeepFromPlayer { get; set; }

    public LayerMask LayerMaskWall { get; set; }
    public LayerMask LayerMaskGrass { get; set; }
}
