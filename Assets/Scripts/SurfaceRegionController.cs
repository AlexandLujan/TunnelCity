using UnityEngine;

public class SurfaceRegionController : MonoBehaviour
{
    [Header("Region")]
    [SerializeField]
    private WorldRegionDefinition regionDefinition;

    [Header("Surface")]
    [SerializeField]
    private SurfaceChunkRenderer surfaceChunkRenderer;

    private SurfaceChunkManager surfaceChunkManager;

    private void Start()
    {
        Debug.Log("SurfaceRegionController started.");

        if (regionDefinition == null)
        {
            Debug.LogError("SurfaceRegionController is missing a WorldRegionDefinition.");
            return;
        }

        if (surfaceChunkRenderer == null)
        {
            Debug.LogError("SurfaceRegionController is missing a SurfaceChunkRenderer.");
            return;
        }

        surfaceChunkManager =
            new SurfaceChunkManager(
                surfaceChunkRenderer,
                regionDefinition
            );

        Debug.Log("SurfaceChunkManager created.");

        surfaceChunkManager.Initialize();

        Debug.Log("SurfaceChunkManager initialized.");

        StreamingManager streamingManager =
            FindAnyObjectByType<StreamingManager>();

        if (streamingManager == null)
        {
            Debug.LogError("No StreamingManager was found.");
            return;
        }

        streamingManager.SetSurfaceChunkManager(surfaceChunkManager);

        Debug.Log("SurfaceChunkManager registered with StreamingManager.");
    }
}