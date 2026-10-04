using System.Collections.Generic;
using UnityEngine;

public class SurfaceChunkManager
{
    private WorldRegionDefinition regionDefinition;
    private static readonly Vector2Int ChunkOrigin = new Vector2Int(-4, -2);
    private SurfaceChunkRenderer renderer;
    private Dictionary<ChunkCoordinate, SurfaceChunkData> chunks;

    public SurfaceChunkManager(SurfaceChunkRenderer surfaceChunkRenderer, WorldRegionDefinition regionDefinition)
    {
        this.renderer = surfaceChunkRenderer;
        this.regionDefinition = regionDefinition;
        chunks = new Dictionary<ChunkCoordinate, SurfaceChunkData>();
    }

    public void Initialize()
    {
        Vector2Int regionSize = regionDefinition.RegionSizeInChunks;

        Debug.Log(
            $"Initializing surface region: {regionSize.x} x {regionSize.y} chunks."
        );

        for (int z = 0; z < regionSize.y; z++)
        {
            for (int x = 0; x < regionSize.x; x++)
            {
                ChunkCoordinate coordinate = new ChunkCoordinate(x, z);
                CaptureChunk(coordinate);
            }
        }

        Debug.Log($"Captured {chunks.Count} surface chunks.");
    }

    // Where is this chunk?
    private BoundsInt GetChunkBounds(ChunkCoordinate coordinate)
    {
        int startX = ChunkOrigin.x + (coordinate.x * regionDefinition.ChunkSize);
        int startY = ChunkOrigin.y + (coordinate.z * regionDefinition.ChunkSize);

        return new BoundsInt(
            startX,
            startY,
            0,
            regionDefinition.ChunkSize,
            regionDefinition.ChunkSize,
            1
        );
    }

    private void CaptureChunk(ChunkCoordinate coordinate)
    {
        BoundsInt bounds = GetChunkBounds(coordinate);

        SurfaceChunkData chunkData = renderer.CaptureChunkData(coordinate, bounds);
        chunks[coordinate] = chunkData;
    }
    private void LoadChunk(ChunkCoordinate coordinate)
    {
        if (!HasChunk(coordinate))
            return;

        SurfaceChunkData chunkData = chunks[coordinate];

        renderer.RenderChunk(chunkData);
    }
    private void UnloadChunk(ChunkCoordinate coordinate)
    {
        if (!HasChunk(coordinate))
            return;

        SurfaceChunkData chunkData = chunks[coordinate];

        renderer.ClearChunk(chunkData.Bounds);
    }
    private bool HasChunk(ChunkCoordinate coordinate)
    {
        return chunks.ContainsKey(coordinate);
    }

    public bool HasChunkData(ChunkCoordinate coordinate)
    {
        return HasChunk(coordinate);
    }

    public void Load(ChunkCoordinate coordinate)
    {
        LoadChunk(coordinate);
    }

    public void Unload(ChunkCoordinate coordinate)
    {
        UnloadChunk(coordinate);
    }
}
