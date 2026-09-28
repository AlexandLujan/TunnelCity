using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private Tilemap wallFaceTilemap;

    [Header("Tileset")]
    [SerializeField]
    private CaveTileSet caveTileSet;

    public void RenderChunk(
        CaveLayerData layer,
        ChunkCoordinate coordinate,
        int chunkSize)
    {
        // Determine the tile bounds for this chunk.

        // Iterate through the cells in that range.

        // Read CaveTileType from layer.tiles.

        // Decide which Tilemap(s) should receive tiles.

        // Use CaveTileSet for the actual Tile assets.
    }

    public void ClearChunk(
        ChunkCoordinate coordinate,
        int chunkSize)
    {
        // Determine the tile bounds for this chunk.

        // Clear the corresponding cells from:
        // FloorTilemap
        // WallTilemap
        // WallFaceTilemap
    }
}
