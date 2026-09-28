using System;
using UnityEngine;

public class CaveDefinition : ScriptableObject
{
    private CaveType caveType;
    private int chunkSize;
    private Vector2Int chunkDimensions;
    private int boundaryThickness;
    private CaveTileSet caveTileSet;

    public int WidthInTiles => chunkDimensions.x * chunkSize;
    public int DepthInTiles => chunkDimensions.y * chunkSize;
}

// Eventually, you'll need to expose the readonly data for CaveManager 
// and CaveGenerator to use.