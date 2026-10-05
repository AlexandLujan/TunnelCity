using System.Collections.Generic;
using UnityEngine;

public class SurfaceChunkManager
{
    private WorldRegionDefinition regionDefinition;

    private SurfaceChunkRenderer[] renderers;

    private Dictionary<
        ChunkCoordinate,
        SurfaceChunkData
    > chunks;

    private Dictionary<
        ChunkCoordinate,
        SurfaceChunkRenderer
    > chunkRenderers;

    private Vector2Int regionOrigin;
    public SurfaceChunkManager(
        SurfaceChunkRenderer[] surfaceChunkRenderers,
        WorldRegionDefinition regionDefinition)
    {
        renderers = surfaceChunkRenderers;
        this.regionDefinition = regionDefinition;

        chunks =
            new Dictionary<
                ChunkCoordinate,
                SurfaceChunkData
            >();

        chunkRenderers =
            new Dictionary<
                ChunkCoordinate,
                SurfaceChunkRenderer
            >();
    }

    public void Initialize()
    {
        Debug.Log(
            $"{regionDefinition.name} WORLD ORIGIN RAW | " +
            $"X: {regionDefinition.WorldOrigin.x} | " +
            $"Layer: {regionDefinition.WorldOrigin.layer} | " +
            $"Z: {regionDefinition.WorldOrigin.z}"
        );

        regionOrigin = new Vector2Int(regionDefinition.WorldOrigin.x, regionDefinition.WorldOrigin.z);

        Vector2Int regionSize =
            regionDefinition.RegionSizeInChunks;

        Debug.Log(
            $"Initializing surface region: " +
            $"{regionSize.x} x {regionSize.y} chunks. " +
            $"Origin: {regionOrigin}"
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
        foreach (SurfaceChunkRenderer renderer
                 in renderers)
        {
            if (renderer == null)
                continue;

            renderer.ClearAll();
        }
    }

    public ChunkCoordinate WorldToChunkCoordinate(
        Vector3 worldPosition)
    {
        float relativeX =
            worldPosition.x - regionOrigin.x;

        float relativeY =
            worldPosition.y - regionOrigin.y;

        int chunkX =
            Mathf.FloorToInt(
                relativeX /
                regionDefinition.ChunkSize
            );

        int chunkZ =
            Mathf.FloorToInt(
                relativeY /
                regionDefinition.ChunkSize
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

    private Vector2Int CalculateRegionOrigin()
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (SurfaceChunkRenderer renderer
                 in renderers)
        {
            if (renderer == null)
                continue;

            BoundsInt bounds =
                renderer.GetCellBounds();

            if (bounds.xMin < minX)
                minX = bounds.xMin;

            if (bounds.yMin < minY)
                minY = bounds.yMin;
        }

        return new Vector2Int(
            minX,
            minY
        );
    }

    private BoundsInt GetChunkBounds(
        ChunkCoordinate coordinate)
    {
        int startX = regionOrigin.x + (coordinate.x * regionDefinition.ChunkSize);

        int startY = regionOrigin.y + (coordinate.z * regionDefinition.ChunkSize);

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

        SurfaceChunkRenderer renderer =
            GetRendererForChunk(bounds);

        if (renderer == null)
            return;

        SurfaceChunkData chunkData =
            renderer.CaptureChunkData(
                coordinate,
                bounds
            );

        chunks[coordinate] =
            chunkData;

        chunkRenderers[coordinate] =
            renderer;
    }

    private void LoadChunk(
        ChunkCoordinate coordinate)
    {
        if (!HasChunk(coordinate))
            return;

        if (!chunkRenderers.TryGetValue(
            coordinate,
            out SurfaceChunkRenderer renderer))
        {
            return;
        }

        SurfaceChunkData chunkData =
            chunks[coordinate];

        renderer.RenderChunk(
            chunkData
        );
    }

    private void UnloadChunk(
        ChunkCoordinate coordinate)
    {
        if (!HasChunk(coordinate))
            return;

        if (!chunkRenderers.TryGetValue(
            coordinate,
            out SurfaceChunkRenderer renderer))
        {
            return;
        }

        SurfaceChunkData chunkData =
            chunks[coordinate];

        renderer.ClearChunk(
            chunkData.Bounds
        );
    }

    private SurfaceChunkRenderer
        GetRendererForChunk(
            BoundsInt bounds)
    {
        foreach (SurfaceChunkRenderer renderer
                 in renderers)
        {
            if (renderer == null)
                continue;

            if (renderer.ContainsChunk(bounds))
                return renderer;
        }

        return null;
    }

    private bool HasChunk(
        ChunkCoordinate coordinate)
    {
        return chunks.ContainsKey(
            coordinate
        );
    }
}