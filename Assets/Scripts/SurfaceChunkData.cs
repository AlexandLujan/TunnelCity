using UnityEngine;
using UnityEngine.Tilemaps;

public class SurfaceChunkData
{
    private ChunkCoordinate coordinate;
    private BoundsInt bounds;

    private TileBase[,] groundTiles;
    private TileBase[,] waterTiles;
    private TileBase[,] detailTiles;
    private TileBase[,] mountainTiles;

    public ChunkCoordinate Coordinate => coordinate;
    public BoundsInt Bounds => bounds;

    public SurfaceChunkData(
        ChunkCoordinate coordinate,
        BoundsInt bounds)
    {
        this.coordinate = coordinate;
        this.bounds = bounds;

        int width = bounds.size.x;
        int height = bounds.size.y;

        groundTiles = new TileBase[width, height];
        waterTiles = new TileBase[width, height];
        detailTiles = new TileBase[width, height];
        mountainTiles = new TileBase[width, height];
    }

    public bool InBounds(Vector3Int cellPosition)
    {
        return bounds.Contains(cellPosition);
    }

    private Vector2Int WorldToLocal(Vector3Int cellPosition)
    {
        return new Vector2Int(
            cellPosition.x - bounds.xMin,
            cellPosition.y - bounds.yMin
        );
    }

    public void SetGroundTile(Vector3Int cellPosition, TileBase tile)
    {
        if (!InBounds(cellPosition))
            return;

        Vector2Int local = WorldToLocal(cellPosition);

        groundTiles[local.x, local.y] = tile;
    }

    public TileBase GetGroundTile(Vector3Int cellPosition)
    {
        if (!InBounds(cellPosition)) return null;

        Vector2Int local = WorldToLocal(cellPosition);

        return groundTiles[local.x, local.y];
    }

    public void SetWaterTile(Vector3Int cellPosition,TileBase tile)
    {
        if (!InBounds(cellPosition)) return;

        Vector2Int local = WorldToLocal(cellPosition);

        waterTiles[local.x, local.y] = tile;
    }

    public TileBase GetWaterTile(Vector3Int cellPosition)
    {
        if (!InBounds(cellPosition)) return null;

        Vector2Int local = WorldToLocal(cellPosition);

        return waterTiles[local.x, local.y];
    }

    public void SetDetailTile(Vector3Int cellPosition, TileBase tile)
    {
        if (!InBounds(cellPosition)) return;

        Vector2Int local = WorldToLocal(cellPosition);

        detailTiles[local.x, local.y] = tile;
    }

    public TileBase GetDetailTile(Vector3Int cellPosition)
    {
        if (!InBounds(cellPosition)) return null;

        Vector2Int local = WorldToLocal(cellPosition);

        return detailTiles[local.x, local.y];
    }

    public void SetMountainTile(Vector3Int cellPosition, TileBase tile)
    {
        if (!InBounds(cellPosition)) return;

        Vector2Int local = WorldToLocal(cellPosition);

        mountainTiles[local.x, local.y] = tile;
    }

    public TileBase GetMountainTile(Vector3Int cellPosition)
    {
        if (!InBounds(cellPosition)) return null;

        Vector2Int local = WorldToLocal(cellPosition);

        return mountainTiles[local.x, local.y];
    }
}
