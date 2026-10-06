public class CaveLayerData
{
    public int layerIndex;
    public int layerSeed;

    public int width;
    public int depth;

    public CaveType caveType;
    public CaveTileType[,] tiles;
    public CaveLayerState state;

    public CaveLayerData(
        int layerIndex,
        int layerSeed,
        int width,
        int depth,
        CaveType caveType)
    {
        this.layerIndex = layerIndex;
        this.layerSeed = layerSeed;
        this.width = width;
        this.depth = depth;
        this.caveType = caveType;

        tiles = new CaveTileType[width, depth];

        // A DataOnly layer begins completely enclosed
        // in RoughStone until generation populates it.
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                tiles[x, z] = CaveTileType.RoughStone;
            }
        }

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
        // Anything outside the cave is treated as
        // permanent boundary stone.
        if (!InBounds(x, z))
            return CaveTileType.RoughStone;

        return tiles[x, z];
    }

    public void SetTile(int x, int z, CaveTileType tileType)
    {
        if (!InBounds(x, z))
            return;

        tiles[x, z] = tileType;
    }

    public bool IsOpen(int x, int z)
    {
        if (!InBounds(x, z))
            return false;

        return tiles[x, z] == CaveTileType.Open;
    }

    public bool IsSolid(int x, int z)
    {
        // Out-of-bounds space is considered solid.
        if (!InBounds(x, z))
            return true;

        return tiles[x, z] != CaveTileType.Open;
    }

    public bool IsMineable(int x, int z)
    {
        if (!InBounds(x, z))
            return false;

        CaveTileType tileType = tiles[x, z];

        // Open space cannot be mined and RoughStone
        // represents the permanent cave boundary.
        return tileType != CaveTileType.Open &&
               tileType != CaveTileType.RoughStone;
    }
}