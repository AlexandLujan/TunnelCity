using UnityEngine;
using UnityEngine.Tilemaps;

public class SurfaceChunkRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap groundTilemap;

    [SerializeField]
    private Tilemap waterTilemap;

    [SerializeField]
    private Tilemap detailTilemap;

    [SerializeField]
    private Tilemap mountainWallsTilemap;

    public BoundsInt GetCellBounds()
    {
        return groundTilemap.cellBounds;
    }

    public bool ContainsChunk(BoundsInt chunkBounds)
    {
        BoundsInt partitionBounds = GetCellBounds();

        return partitionBounds.xMin < chunkBounds.xMax &&
               partitionBounds.xMax > chunkBounds.xMin &&
               partitionBounds.yMin < chunkBounds.yMax &&
               partitionBounds.yMax > chunkBounds.yMin;
    }

    public SurfaceChunkData CaptureChunkData(
    ChunkCoordinate coordinate,
    BoundsInt bounds)
    {
        SurfaceChunkData chunkData =
            new SurfaceChunkData(coordinate, bounds);

        int capturedGroundTiles = 0;

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            TileBase groundTile =
                groundTilemap.GetTile(cellPosition);

            if (groundTile != null)
                capturedGroundTiles++;

            chunkData.SetGroundTile(
                cellPosition,
                groundTile
            );

            chunkData.SetWaterTile(
                cellPosition,
                waterTilemap.GetTile(cellPosition)
            );

            chunkData.SetDetailTile(
                cellPosition,
                detailTilemap.GetTile(cellPosition)
            );

            chunkData.SetMountainTile(
                cellPosition,
                mountainWallsTilemap.GetTile(cellPosition)
            );
        }

        return chunkData;
    }

    public void RenderChunk(SurfaceChunkData chunkData)
    {
        int renderedGroundTiles = 0;

        foreach (Vector3Int cellPosition
                 in chunkData.Bounds.allPositionsWithin)
        {
            TileBase groundTile =
                chunkData.GetGroundTile(cellPosition);

            if (groundTile != null)
                renderedGroundTiles++;

            groundTilemap.SetTile(
                cellPosition,
                groundTile
            );

            waterTilemap.SetTile(
                cellPosition,
                chunkData.GetWaterTile(cellPosition)
            );

            detailTilemap.SetTile(
                cellPosition,
                chunkData.GetDetailTile(cellPosition)
            );

            mountainWallsTilemap.SetTile(
                cellPosition,
                chunkData.GetMountainTile(cellPosition)
            );
        }
    }

    public void ClearChunk(BoundsInt bounds)
    {
        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            groundTilemap.SetTile(cellPosition, null);
            waterTilemap.SetTile(cellPosition, null);
            detailTilemap.SetTile(cellPosition, null);
            mountainWallsTilemap.SetTile(cellPosition, null);
        }
    }

    public void ClearAll()
    {
        groundTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();
        detailTilemap.ClearAllTiles();
        mountainWallsTilemap.ClearAllTiles();
    }
}