using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StreamingManager : MonoBehaviour
{
    [Header("Chunk Streaming")]
    [SerializeField]
    private int streamingRadius = 1;

    [Header("Core References")]
    [SerializeField]
    private CardinalMovement cardinalMovement;

    [SerializeField]
    private Grid overworldGrid;

    private WorldRegionDefinition currentRegion;

    private ChunkCoordinate currentChunk;

    private HashSet<ChunkCoordinate> activeChunks;

    private SurfaceChunkManager surfaceChunkManager;

    private Dictionary<
        WorldRegionDefinition,
        SurfaceChunkManager
    > surfaceRegions;

    private int currentLayerIndex;

    private void Awake()
    {
        activeChunks =
            new HashSet<
                ChunkCoordinate
            >();

        surfaceRegions =
            new Dictionary<
                WorldRegionDefinition,
                SurfaceChunkManager
            >();
    }

    private void Start()
    {
        if (cardinalMovement == null)
            return;

        if (overworldGrid == null)
            return;

        cardinalMovement.SetWorldGrid(
            overworldGrid
        );

        TryActivateRegionForPlayer();
    }

    private void Update()
    {
        if (cardinalMovement == null)
            return;

        if (surfaceChunkManager == null)
        {
            TryActivateRegionForPlayer();

            return;
        }

        ChunkCoordinate playerChunk =
            surfaceChunkManager.WorldToChunkCoordinate(
                cardinalMovement.transform.position
            );

        Debug.Log(
            $"CURRENT REGION: {currentRegion?.name} | " +
            $"PLAYER CHUNK: {playerChunk} | " +
            $"HAS DATA: {surfaceChunkManager.HasChunkData(playerChunk)}"
        );

        // Player has moved outside the
        // currently active region.
        if (!surfaceChunkManager.HasChunkData(
                playerChunk))
        {
            TryActivateRegionForPlayer();

            return;
        }

        UpdateCurrentChunk(
            playerChunk
        );
    }

    public void RegisterSurfaceRegion(
        WorldRegionDefinition regionDefinition,
        SurfaceChunkManager chunkManager)
    {
        if (regionDefinition == null)
            return;

        if (chunkManager == null)
            return;

        surfaceRegions[regionDefinition] =
            chunkManager;

        if (chunkManager.CapturedChunkCount == 0)
        {
            Debug.LogError(
                $"Surface region " +
                $"{regionDefinition.name} " +
                $"captured ZERO chunks. " +
                $"Tilemaps will NOT be cleared."
            );

            return;
        }

        Debug.Log(
            $"Surface region " +
            $"{regionDefinition.name} " +
            $"registered with " +
            $"{chunkManager.CapturedChunkCount} " +
            $"captured chunks."
        );

        // The manager has already captured
        // its painted data.
        //
        // Clear the physical Tilemaps so
        // chunks can now be streamed.
        chunkManager.PrepareForStreaming();

        if (cardinalMovement == null)
            return;

        ChunkCoordinate playerChunk =
            chunkManager.WorldToChunkCoordinate(
                cardinalMovement.transform.position
            );

        // If the player is currently inside
        // this region, make it the active
        // surface region.
        if (chunkManager.HasChunkData(
                playerChunk))
        {
            ActivateSurfaceRegion(
                regionDefinition,
                chunkManager
            );
        }
    }

    private void ActivateSurfaceRegion(
        WorldRegionDefinition regionDefinition,
        SurfaceChunkManager chunkManager)
    {
        if (regionDefinition == null)
            return;

        if (chunkManager == null)
            return;

        if (currentRegion == regionDefinition &&
            surfaceChunkManager == chunkManager)
        {
            return;
        }

        // Unload chunks from the previously
        // active region.
        if (surfaceChunkManager != null)
        {
            foreach (
                ChunkCoordinate coordinate
                in activeChunks)
            {
                surfaceChunkManager.Unload(
                    coordinate
                );
            }
        }

        activeChunks.Clear();

        currentRegion =
            regionDefinition;

        surfaceChunkManager =
            chunkManager;

        Debug.Log(
            $"Active surface region changed to: " +
            $"{regionDefinition.name}"
        );

        if (cardinalMovement != null &&
            overworldGrid != null)
        {
            cardinalMovement.SetWorldGrid(
                overworldGrid
            );
        }

        if (cardinalMovement == null)
            return;

        ChunkCoordinate startingChunk =
            surfaceChunkManager
                .WorldToChunkCoordinate(
                    cardinalMovement
                        .transform
                        .position
                );

        InitializeChunks(
            startingChunk
        );
    }

    private void TryActivateRegionForPlayer()
    {
        if (cardinalMovement == null)
            return;

        foreach (
            KeyValuePair<
                WorldRegionDefinition,
                SurfaceChunkManager
            > region in surfaceRegions)
        {
            SurfaceChunkManager chunkManager =
                region.Value;

            if (chunkManager == null)
                continue;

            ChunkCoordinate playerChunk =
                chunkManager
                    .WorldToChunkCoordinate(
                        cardinalMovement
                            .transform
                            .position
                    );

            Debug.Log(
                $"CHECK REGION: " +
                $"{region.Key.name} | " +
                $"Chunk: {playerChunk} | " +
                $"Valid: " +
                $"{chunkManager.HasChunkData(playerChunk)}"
            );

            if (!chunkManager.HasChunkData(
                    playerChunk))
            {
                continue;
            }

            ActivateSurfaceRegion(
                region.Key,
                chunkManager
            );

            return;
        }
    }

    public void SetCurrentLayer(
        int layerIndex)
    {
        currentLayerIndex =
            layerIndex;
    }

    public async void LoadRegion(
        WorldRegionDefinition regionDefinition)
    {
        if (regionDefinition == null)
            return;

        if (string.IsNullOrEmpty(
                regionDefinition.SceneName))
        {
            return;
        }

        Scene existingScene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (existingScene.isLoaded)
        {
            if (cardinalMovement != null &&
                overworldGrid != null)
            {
                cardinalMovement.SetWorldGrid(
                    overworldGrid
                );
            }

            TryActivateRegionForPlayer();

            return;
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                regionDefinition.SceneName,
                LoadSceneMode.Additive
            );

        if (operation == null)
            return;

        while (!operation.isDone)
            await Task.Yield();

        Scene loadedScene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (!loadedScene.isLoaded)
            return;

        if (cardinalMovement != null &&
            overworldGrid != null)
        {
            cardinalMovement.SetWorldGrid(
                overworldGrid
            );
        }

        TryActivateRegionForPlayer();
    }

    public async void UnloadRegion(
        WorldRegionDefinition regionDefinition)
    {
        if (regionDefinition == null)
            return;

        if (string.IsNullOrEmpty(
                regionDefinition.SceneName))
        {
            return;
        }

        Scene scene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (!scene.isLoaded)
            return;

        bool unloadingCurrentRegion =
            currentRegion ==
            regionDefinition;

        AsyncOperation operation =
            SceneManager.UnloadSceneAsync(
                scene
            );

        if (operation == null)
            return;

        while (!operation.isDone)
            await Task.Yield();

        surfaceRegions.Remove(
            regionDefinition
        );

        if (unloadingCurrentRegion)
        {
            currentRegion = null;

            surfaceChunkManager = null;

            activeChunks.Clear();

            TryActivateRegionForPlayer();
        }
    }

    public void InitializeChunks(
        ChunkCoordinate startingChunk)
    {
        currentChunk =
            startingChunk;

        RefreshChunks();
    }

    public void UpdateCurrentChunk(
        ChunkCoordinate coordinate)
    {
        if (coordinate ==
            currentChunk)
        {
            return;
        }

        currentChunk =
            coordinate;

        RefreshChunks();
    }

    private void RefreshChunks()
    {
        if (surfaceChunkManager == null)
            return;

        HashSet<ChunkCoordinate>
            desiredChunks =
                GetDesiredChunks();

        foreach (
            ChunkCoordinate coordinate
            in desiredChunks)
        {
            if (!activeChunks.Contains(
                    coordinate))
            {
                Debug.Log(
                    $"Loading surface chunk: " +
                    $"{coordinate}"
                );

                surfaceChunkManager.Load(
                    coordinate
                );
            }
        }

        foreach (
            ChunkCoordinate coordinate
            in activeChunks)
        {
            if (!desiredChunks.Contains(
                    coordinate))
            {
                surfaceChunkManager.Unload(
                    coordinate
                );
            }
        }

        activeChunks =
            desiredChunks;
    }

    private HashSet<ChunkCoordinate>
        GetDesiredChunks()
    {
        HashSet<ChunkCoordinate>
            desiredChunks =
                new HashSet<
                    ChunkCoordinate
                >();

        if (surfaceChunkManager == null)
            return desiredChunks;

        for (int z = -streamingRadius;
             z <= streamingRadius;
             z++)
        {
            for (int x = -streamingRadius;
                 x <= streamingRadius;
                 x++)
            {
                ChunkCoordinate coordinate =
                    new ChunkCoordinate(
                        currentChunk.x + x,
                        currentChunk.z + z
                    );

                if (!surfaceChunkManager
                    .HasChunkData(
                        coordinate))
                {
                    continue;
                }

                desiredChunks.Add(
                    coordinate
                );
            }
        }

        return desiredChunks;
    }
}