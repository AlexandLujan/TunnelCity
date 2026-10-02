using System.Collections;

public class CaveLayerData
{
    public int layerIndex;
    public int layerSeed;

    public int width;
    public int depth;

    public CaveType caveType;
    public CaveTileType[,] tiles;
    public CaveLayerState state;

    public CaveLayerData(int layerIndex, int layerSeed, int width, int depth, CaveType caveType)
    {
        this.layerIndex = layerIndex;
        this.layerSeed = layerSeed;
        this.width = width;
        this.depth = depth;
        this.caveType = caveType;

        tiles = new CaveTileType[width, depth];
        state = CaveLayerState.DataOnly;
    }

    public bool InBounds(int x, int z)
    {
        return x >= 0 &&
            x < width &&
            z >= 0 &&
            z < depth;
    }

    public CaveTileType GetTile(int x, int z)
    {
        if (!InBounds(x, z)) return default;
        return tiles[x,z];
    }

    public void SetTile(int x, int z, CaveTileType tileType)
    {
        if (!InBounds(x, z)) return;
        tiles[x, z] = tileType;
    }
}