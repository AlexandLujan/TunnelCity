using System.Collections;
using UnityEngine;

public class WildernessChunkData
{
    public ChunkCoordinate coordinate;

    public int width;
    public int depth;

    public bool isLoaded;

    public WildernessChunkData(
        ChunkCoordinate coordinate,
        int width,
        int depth)
    {
        this.coordinate = coordinate;
        this.width = width;
        this.depth = depth;

        isLoaded = false;
    }
}