using System.Collections.Generic;
using UnityEngine;

public class SurfaceChunkManager
{
    private const int ChunkSize = 32;
    private static readonly Vector2Int ChunkOrigin = new Vector2Int(-4, -2);
    private SurfaceChunkRenderer reference;
    private Dictionary<ChunkCoordinate, SurfaceChunkData> chunks;

    public SurfaceChunkManager(SurfaceChunkRenderer surfaceChunkRenderer)
    {
        this.reference = surfaceChunkRenderer;
        chunks = new Dictionary<ChunkCoordinate, SurfaceChunkData>();
    }

    // Where is this chunk?
    private BoundsInt GetChunkBounds(ChunkCoordinate coordinate)
    {
        int startX = ChunkOrigin.x + (coordinate.x * ChunkSize);
        int startY = ChunkOrigin.y + (coordinate.z * ChunkSize);

        return new BoundsInt(
            startX,
            startY,
            0,
            ChunkSize,
            ChunkSize,
            1
        );
    }

    private void CaptureChunk(ChunkCoordinate coordinate)
    {
        BoundsInt bounds = GetChunkBounds(coordinate);

        SurfaceChunkData chunkData = reference.CaptureChunkData(coordinate, bounds);
        chunks[coordinate] = chunkData;
    }
    private void LoadChunk(ChunkCoordinate coordinate)
    {

    }
    private void UnloadChunk(ChunkCoordinate coordinate)
    {

    }
    private bool HasChunk(ChunkCoordinate coordinate)
    {
        return false;
    }
}
