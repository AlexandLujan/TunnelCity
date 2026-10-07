using System.Collections.Generic;
using UnityEngine;

public class SurfaceRegionController : MonoBehaviour
{
    [Header("Region")]
    [SerializeField]
    private WorldRegionDefinition regionDefinition;

    private SurfaceChunkManager surfaceChunkManager;

    private void Start()
    {
        Debug.Log($"REGION CONTROLLER START | Scene: {gameObject.scene.name}");
        if (regionDefinition == null)
        {
            Debug.LogError(
                "SurfaceRegionController is missing a WorldRegionDefinition."
            );

            return;
        }

        SurfaceChunkRenderer[] allRenderers =
            FindObjectsByType<SurfaceChunkRenderer>();

        List<SurfaceChunkRenderer> regionRenderers =
            new List<SurfaceChunkRenderer>();

        foreach (SurfaceChunkRenderer renderer in allRenderers)
        {
            if (renderer.gameObject.scene == gameObject.scene)
            {
                regionRenderers.Add(renderer);
            }
        }

        SurfaceChunkRenderer[] surfaceChunkRenderers =
            regionRenderers.ToArray();

        if (surfaceChunkRenderers.Length == 0)
        {
            Debug.LogError(
                $"No SurfaceChunkRenderers were found for " +
                $"{regionDefinition.name}."
            );

            return;
        }

        Debug.Log(
            $"Found {surfaceChunkRenderers.Length} " +
            $"SurfaceChunkRenderer(s) for " +
            $"{regionDefinition.name}."
        );

        surfaceChunkManager =
            new SurfaceChunkManager(
                surfaceChunkRenderers,
                regionDefinition
            );

        surfaceChunkManager.Initialize();

        StreamingManager streamingManager =
            FindAnyObjectByType<StreamingManager>();

        if (streamingManager == null)
        {
            Debug.LogError(
                "No StreamingManager was found."
            );

            return;
        }

        streamingManager.RegisterSurfaceRegion(
            regionDefinition,
            surfaceChunkManager
        );

        Debug.Log(
            $"SurfaceChunkManager registered for " +
            $"{regionDefinition.name}."
        );
    }
}