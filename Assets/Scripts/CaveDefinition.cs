using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCaveDefinition",
    menuName = "Cave/Cave Definition")]
public class CaveDefinition : ScriptableObject
{
    private CaveType caveType;
    private int chunkSize;
    private Vector2Int chunkDimensions;
    private int boundaryThickness;
    private CaveTileSet caveTileSet;

    public CaveType CaveType => caveType;
    public int ChunkSize => chunkSize;
    public Vector2Int ChunkDimensions => chunkDimensions;
    public int BoundaryThickness => boundaryThickness;
    public CaveTileSet CaveTileSet => caveTileSet;
    public int WidthInTiles => chunkDimensions.x * chunkSize;
    public int DepthInTiles => chunkDimensions.y * chunkSize;
}

// Eventually, you'll need to expose the readonly data for CaveManager 
// and CaveGenerator to use.