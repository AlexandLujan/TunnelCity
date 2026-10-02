using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

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

    public void RenderChunk(
        ChunkCoordinate coordinate,
        int chunkSize)
    {
        int xTemp = coordinate.x;
        int zTemp = coordinate.z;
        Vector2Int cellPosition = new Vector2Int(xTemp, zTemp);

        //1.Receive a ChunkCoordinate
        //2.Convert that chunk coordinate into Tilemap cell bounds
        //3.Find the Wilderness tiles / content that belong inside those bounds
        //4.Make that content active / visible
    }

    public void ClearChunk(
        ChunkCoordinate coordinate,
        int chunkSize)
    {
        // Clear or disable cells/content
        // belonging to this chunk.
    }
}

/*
Tilemap.GetTile(cellPosition)
Tilemap.SetTile(cellPosition, tile)
Tilemap.HasTile(cellPosition)
Tilemap.WorldToCell(worldPosition)
Tilemap.CellToWorld(cellPosition) 
*/ 