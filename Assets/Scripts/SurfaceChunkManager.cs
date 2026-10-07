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

    // Origin of the actual painted Tilemaps.
    // Used when capturing/rendering chunk data.
    private Vector2Int tilemapOrigin;

    // Logical global origin of this region.
    // Used when determining which chunk the player occupies.
    private Vector2Int worldOrigin;

    public int CapturedChunkCount =>
        chunks.Count;

    public SurfaceChunkManager(
        SurfaceChunkRenderer[] surfaceChunkRenderers,
        WorldRegionDefinition regionDefinition)
    {
        renderers =
            surfaceChunkRenderers;

        this.regionDefinition =
            regionDefinition;

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
        // Physical Tilemap coordinate origin.
        tilemapOrigin = regionDefinition.TilemapOrigin;

        // Logical/global world origin.
        worldOrigin =
            new Vector2Int(
                regionDefinition.WorldOrigin.x,
                regionDefinition.WorldOrigin.z
            );

        Vector2Int regionSize =
            regionDefinition.RegionSizeInChunks;

        Debug.Log(
            $"Initializing surface region: " +
            $"{regionSize.x} x {regionSize.y} chunks. " +
            $"Tilemap Origin: {tilemapOrigin} | " +
            $"World Origin: {worldOrigin}"
        );

        for (int z = 0;
             z < regionSize.y;
             z++)
        {
            for (int x = 0;
                 x < regionSize.x;
                 x++)
            {
                ChunkCoordinate coordinate =
                    new ChunkCoordinate(
                        x,
                        z
                    );

                CaptureChunk(
                    coordinate
                );
            }
        }

        Debug.Log(
            $"Captured {chunks.Count} chunks for " +
            $"{regionDefinition.name}."
        );
    }

    public void PrepareForStreaming()
    {
        foreach (
            SurfaceChunkRenderer renderer
            in renderers)
        {
            if (renderer == null)
                continue;

            renderer.ClearAll();
        }
    }

    public ChunkCoordinate WorldToChunkCoordinate(
    Vector3Int globalCellPosition)
    {
        int relativeX =
            globalCellPosition.x -
            worldOrigin.x;

        int relativeZ =
            globalCellPosition.y -
            worldOrigin.y;

        int chunkX =
            Mathf.FloorToInt(
                (float)relativeX /
                regionDefinition.ChunkSize
            );

        int chunkZ =
            Mathf.FloorToInt(
                (float)relativeZ /
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
        return HasChunk(
            coordinate
        );
    }

    public void Load(
        ChunkCoordinate coordinate)
    {
        LoadChunk(
            coordinate
        );
    }

    public void Unload(
        ChunkCoordinate coordinate)
    {
        UnloadChunk(
            coordinate
        );
    }

    private Vector2Int CalculateRegionOrigin()
    {
        int minX =
            int.MaxValue;

        int minY =
            int.MaxValue;

        foreach (
            SurfaceChunkRenderer renderer
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
        // IMPORTANT:
        // Capture/render coordinates are based
        // on the actual Tilemap origin, NOT the
        // region's logical world origin.

        int startX =
            tilemapOrigin.x +
            (
                coordinate.x *
                regionDefinition.ChunkSize
            );

        int startY =
            tilemapOrigin.y +
            (
                coordinate.z *
                regionDefinition.ChunkSize
            );

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
            GetChunkBounds(
                coordinate
            );

        if (coordinate.x == 6 &&
    coordinate.z == 0)
        {
            Debug.Log(
                $"TARGET CAPTURE CHECK | " +
                $"Region: {regionDefinition.name} | " +
                $"Chunk: {coordinate} | " +
                $"Bounds: {bounds} | " +
                $"Tilemap Origin: {tilemapOrigin} | " +
                $"World Origin: {worldOrigin}"
            );

            foreach (SurfaceChunkRenderer testRenderer
                     in renderers)
            {
                if (testRenderer == null)
                    continue;

                Debug.Log(
                    $"TARGET RENDERER CHECK | " +
                    $"Region: {regionDefinition.name} | " +
                    $"Renderer: {testRenderer.name} | " +
                    $"Tiles In Bounds: " +
                    $"{testRenderer.CountTilesInBounds(bounds)} | " +
                    $"Renderer Bounds: {testRenderer.GetCellBounds()}"
                );
            }
        }

        SurfaceChunkRenderer renderer =
            GetRendererForChunk(
                bounds
            );

        // An empty/unpainted chunk is allowed.
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
        if (!HasChunk(
                coordinate))
        {
            return;
        }

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
        if (!HasChunk(
                coordinate))
        {
            return;
        }

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
        SurfaceChunkRenderer bestRenderer =
            null;

        int bestTileCount =
            0;

        foreach (
            SurfaceChunkRenderer renderer
            in renderers)
        {
            if (renderer == null)
                continue;

            int tileCount =
                renderer.CountTilesInBounds(
                    bounds
                );

            if (tileCount <=
                bestTileCount)
            {
                continue;
            }

            bestTileCount =
                tileCount;

            bestRenderer =
                renderer;
        }

        return bestRenderer;
    }

    private bool HasChunk(
        ChunkCoordinate coordinate)
    {
        return chunks.ContainsKey(
            coordinate
        );
    }
}