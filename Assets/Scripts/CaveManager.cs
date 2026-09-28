using UnityEngine;

public class CaveManager : MonoBehaviour
{
    [Header("Cave Systems")]
    [SerializeField]
    private CaveGenerator caveGenerator;

    [SerializeField]
    private CaveChunkManager chunkManager;

    [SerializeField]
    private CaveRenderer caveRenderer;

    [Header("Cave Definition")]
    [SerializeField]
    private CaveDefinition caveDefinition;

    private CaveData currentCave;
    private CaveLayerData currentLayer;

    public CaveData CurrentCave => currentCave;
    public CaveLayerData CurrentLayer => currentLayer;

    private void Awake()
    {
        InitializeCave();
    }

    private void InitializeCave()
    {
        // Create the runtime CaveData object.
        // Store the CaveDefinition.
        // Initialize the cave seed.
        // Create the DataOnly layers.
    }

    public void LoadLayer(int layerIndex)
    {
        // Find the requested layer.

        // If the layer is DataOnly:
        //     do NOT automatically generate it here
        //     unless generation is explicitly requested.

        // Cache/deactivate the current layer.

        // Set the requested layer as current.

        // Tell the chunk manager which layer is active.

        // Tell the renderer to display the active layer/chunks.
    }

    public void GenerateLayer(int layerIndex)
    {
        // Find the requested CaveLayerData.

        // If it has already been generated:
        //     return.

        // Ask CaveGenerator to carve the layer.

        // Change its state from DataOnly -> Generated.
    }

    public void BlastEntrance(DrilledOpening opening)
    {
        // Determine the target layer from the opening direction.

        // Determine what exists at the destination.

        // If destination is normal procedural cave:
        //     GenerateLayer(targetLayerIndex)
        //     Create CaveEntrance.

        // If destination is a named location:
        //     Create LocationEntrance.

        // Mark the DrilledOpening as blasted.
    }

    public void TravelThroughEntrance(CaveEntrance entrance)
    {
        // Determine which side of the entrance the player is currently on.

        // Load the opposite layer.

        // Move the player to the corresponding entrance position.
    }
}
