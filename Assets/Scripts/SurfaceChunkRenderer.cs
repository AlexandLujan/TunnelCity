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

        Vector3Int centerCell = new Vector3Int(
            chunkBounds.xMin + (chunkBounds.size.x / 2),
            chunkBounds.yMin + (chunkBounds.size.y / 2),
            0
        );

        return partitionBounds.Contains(centerCell);
    }

    public SurfaceChunkData CaptureChunkData(
    ChunkCoordinate coordinate,
    BoundsInt bounds)
    {
        SurfaceChunkData chunkData =
            new SurfaceChunkData(
                coordinate,
                bounds
            );

        int capturedGroundTiles = 0;
        int capturedWaterTiles = 0;
        int capturedDetailTiles = 0;
        int capturedMountainTiles = 0;

        foreach (Vector3Int cellPosition
                 in bounds.allPositionsWithin)
        {
            TileBase groundTile =
                groundTilemap.GetTile(
                    cellPosition
                );

            TileBase waterTile =
                waterTilemap.GetTile(
                    cellPosition
                );

            TileBase detailTile =
                detailTilemap.GetTile(
                    cellPosition
                );

            TileBase mountainTile =
                mountainWallsTilemap.GetTile(
                    cellPosition
                );

            if (groundTile != null)
                capturedGroundTiles++;

            if (waterTile != null)
                capturedWaterTiles++;

            if (detailTile != null)
                capturedDetailTiles++;

            if (mountainTile != null)
                capturedMountainTiles++;

            chunkData.SetGroundTile(
                cellPosition,
                groundTile
            );

            chunkData.SetWaterTile(
                cellPosition,
                waterTile
            );

            chunkData.SetDetailTile(
                cellPosition,
                detailTile
            );

            chunkData.SetMountainTile(
                cellPosition,
                mountainTile
            );
        }

        Debug.Log(
            $"CAPTURE {coordinate} | " +
            $"Renderer: {name} | " +
            $"Bounds: {bounds} | " +
            $"Ground: {capturedGroundTiles} | " +
            $"Water: {capturedWaterTiles} | " +
            $"Detail: {capturedDetailTiles} | " +
            $"Mountain: {capturedMountainTiles}"
        );

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

    public int CountTilesInBounds(BoundsInt bounds)
    {
        int tileCount = 0;

        foreach (Vector3Int cellPosition
                 in bounds.allPositionsWithin)
        {
            if (groundTilemap.GetTile(cellPosition) != null)
                tileCount++;

            if (waterTilemap.GetTile(cellPosition) != null)
                tileCount++;

            if (detailTilemap.GetTile(cellPosition) != null)
                tileCount++;

            if (mountainWallsTilemap.GetTile(cellPosition) != null)
                tileCount++;
        }

        return tileCount;
    }

    public Vector3Int WorldToCell(
    Vector3 worldPosition)
    {
        return groundTilemap.WorldToCell(
            worldPosition
        );
    }
}