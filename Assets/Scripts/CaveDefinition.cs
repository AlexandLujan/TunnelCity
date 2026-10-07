using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCaveDefinition",
    menuName = "Cave/Cave Definition")]
public class CaveDefinition : ScriptableObject
{
    [Header("Cave Type")]
    [SerializeField]
    private CaveType caveType;

    [Header("Dimensions")]
    [SerializeField]
    [Min(1)]
    private int chunkSize = 16;

    [SerializeField]
    private Vector2Int chunkDimensions = Vector2Int.one;

    [SerializeField]
    [Min(1)]
    private int boundaryThickness = 1;

    [Header("Generation")]
    [SerializeField]
    [Range(0.05f, 0.95f)]
    private float carveDensity = 0.35f;

    [SerializeField]
    [Range(0f, 1f)]
    private float chamberChance = 0.15f;

    [Header("Tileset")]
    [SerializeField]
    private CaveTileSet caveTileSet;

    public CaveType CaveType => caveType;
    public int ChunkSize => chunkSize;
    public Vector2Int ChunkDimensions => chunkDimensions;
    public int BoundaryThickness => boundaryThickness;
    public float CarveDensity => carveDensity;
    public float ChamberChance => chamberChance;
    public CaveTileSet CaveTileSet => caveTileSet;
    public int WidthInTiles => chunkDimensions.x * chunkSize;
    public int DepthInTiles => chunkDimensions.y * chunkSize;
}
