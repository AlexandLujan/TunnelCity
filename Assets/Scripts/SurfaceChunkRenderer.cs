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

    public SurfaceChunkData CaptureChunkData(
        ChunkCoordinate coordinate,
        BoundsInt bounds)
    {
        SurfaceChunkData chunkData =
            new SurfaceChunkData(coordinate, bounds);

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            chunkData.SetGroundTile(
                cellPosition,
                groundTilemap.GetTile(cellPosition)
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
        foreach (Vector3Int cellPosition
                 in chunkData.Bounds.allPositionsWithin)
        {
            groundTilemap.SetTile(
                cellPosition,
                chunkData.GetGroundTile(cellPosition)
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