using System.Collections;

public class CaveLayerData
{
    public int layerIndex;
    public int layerSeed;

    public int width;
    public int depth;

    public CaveTileType[,] tiles;
    public CaveLayerState state;

    public CaveLayerData(int layerIndex, int layerSeed, int width,int depth)
    {
        this.layerIndex = layerIndex;
        this.layerSeed = layerSeed;
        this.width = width;
        this.depth = depth;

        tiles = new CaveTileType[width, depth];
        state = CaveLayerState.DataOnly;
    }

    public bool InBounds(int x, int z)
    {
        // Check whether the requested tile coordinate
        // exists within this cave layer.

        return false;
    }

    public CaveTileType GetTile(int x, int z)
    {
        // Return the tile type at the requested coordinate.

        return default;
    }

    public void SetTile(int x, int z, CaveTileType tileType)
    {
        // Change the tile at the requested coordinate.
    }
}