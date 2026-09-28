using UnityEngine;

public class CaveChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField]
    private int renderDistance = 1;

    private CaveLayerData currentLayer;

    private ChunkCoordinate currentChunk;

    public CaveLayerData CurrentLayer => currentLayer;

    public ChunkCoordinate CurrentChunk => currentChunk;

    public void SetLayer(CaveLayerData layer)
    {
        // Store the active layer.

        // Reset the current chunk if necessary.

        // Determine which chunks should initially be active.
    }

    public void UpdateCurrentChunk(Vector2Int playerTilePosition)
    {
        // Convert the player's tile position
        // into a ChunkCoordinate.

        // If the player entered a new chunk:
        // update currentChunk
        // refresh active chunks.
    }

    private void RefreshActiveChunks()
    {
        // Determine which chunks fall within renderDistance.

        // Request newly-needed chunks to be rendered.

        // Request distant chunks to be cleared/unloaded.
    }

    public bool IsChunkInBounds(ChunkCoordinate coordinate)
    {
        // Determine whether the requested chunk
        // actually exists inside currentLayer.

        return false;
    }
}