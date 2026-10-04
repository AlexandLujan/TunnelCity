using System.Collections.Generic;
using UnityEngine;

public class SurfaceChunkManager
{
    private WorldRegionDefinition regionDefinition;

    private static readonly Vector2Int chunkOrigin =
        new Vector2Int(-4, -2);

    private SurfaceChunkRenderer renderer;

    private Dictionary<ChunkCoordinate, SurfaceChunkData> chunks;

    public SurfaceChunkManager(
        SurfaceChunkRenderer surfaceChunkRenderer,
        WorldRegionDefinition regionDefinition)
    {
        renderer = surfaceChunkRenderer;
        this.regionDefinition = regionDefinition;

        chunks =
            new Dictionary<
                ChunkCoordinate,
                SurfaceChunkData
            >();
    }

    public void Initialize()
    {
        Vector2Int regionSize =
            regionDefinition.RegionSizeInChunks;

        Debug.Log(
            $"Initializing surface region: " +
            $"{regionSize.x} x {regionSize.y} chunks."
        );

        for (int z = 0; z < regionSize.y; z++)
        {
            for (int x = 0; x < regionSize.x; x++)
            {
                ChunkCoordinate coordinate =
                    new ChunkCoordinate(x, z);

                CaptureChunk(coordinate);
            }
        }

        Debug.Log(
            $"Captured {chunks.Count} surface chunks."
        );
    }

    public void PrepareForStreaming()
    {
        renderer.ClearAll();
    }

    public ChunkCoordinate WorldToChunkCoordinate(
        Vector3 worldPosition)
    {
        float relativeX =
            worldPosition.x - chunkOrigin.x;

        float relativeY =
            worldPosition.y - chunkOrigin.y;

        int chunkX = Mathf.FloorToInt(
            relativeX / regionDefinition.ChunkSize
        );

        int chunkZ = Mathf.FloorToInt(
            relativeY / regionDefinition.ChunkSize
        );

        return new ChunkCoordinate(
            chunkX,
            chunkZ
        );
    }

    public bool HasChunkData(
        ChunkCoordinate coordinate)
    {
        return HasChunk(coordinate);
    }

    public void Load(
        ChunkCoordinate coordinate)
    {
        LoadChunk(coordinate);
    }

    public void Unload(
        ChunkCoordinate coordinate)
    {
        UnloadChunk(coordinate);
    }

    private BoundsInt GetChunkBounds(
        ChunkCoordinate coordinate)
    {
        int startX =
            chunkOrigin.x +
            (coordinate.x * regionDefinition.ChunkSize);

        int startY =
            chunkOrigin.y +
            (coordinate.z * regionDefinition.ChunkSize);

        return new BoundsInt(
            startX,
            startY,
            0,
            regionDefinition.ChunkSize,
            regionDefinition.ChunkSize,
            1
        );
    }

    private void CaptureChunk(
        ChunkCoordinate coordinate)
    {
        BoundsInt bounds =
            GetChunkBounds(coordinate);

        SurfaceChunkData chunkData =
            renderer.CaptureChunkData(
                coordinate,
                bounds
            );

        chunks[coordinate] = chunkData;
    }

    private void LoadChunk(
        ChunkCoordinate coordinate)
    {
        if (!HasChunk(coordinate))
            return;

        SurfaceChunkData chunkData =
            chunks[coordinate];

        renderer.RenderChunk(chunkData);
    }

    private void UnloadChunk(
        ChunkCoordinate coordinate)
    {
        if (!HasChunk(coordinate))
            return;

        SurfaceChunkData chunkData =
            chunks[coordinate];

        renderer.ClearChunk(
            chunkData.Bounds
        );
    }

    private bool HasChunk(
        ChunkCoordinate coordinate)
    {
        return chunks.ContainsKey(coordinate);
    }
}