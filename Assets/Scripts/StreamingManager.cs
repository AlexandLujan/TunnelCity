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

    [SerializeField]
    private WorldManager worldManager;

    private WorldRegionDefinition currentRegion;

    private Dictionary<
        WorldRegionDefinition,
        SurfaceChunkManager
    > surfaceRegions;

    private Dictionary<
        WorldRegionDefinition,
        HashSet<ChunkCoordinate>
    > activeChunksByRegion;

    private int currentLayerIndex;

    private void Awake()
    {
        surfaceRegions =
            new Dictionary<
                WorldRegionDefinition,
                SurfaceChunkManager
            >();

        activeChunksByRegion =
            new Dictionary<
                WorldRegionDefinition,
                HashSet<ChunkCoordinate>
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

        UpdateCurrentGameplayRegion();

        RefreshAllSurfaceRegions();
    }

    private void Update()
    {
        if (cardinalMovement == null)
            return;

        if (overworldGrid == null)
            return;

        UpdateCurrentGameplayRegion();

        RefreshAllSurfaceRegions();
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

        if (!activeChunksByRegion.ContainsKey(
                regionDefinition))
        {
            activeChunksByRegion[
                regionDefinition
            ] =
                new HashSet<
                    ChunkCoordinate
                >();
        }

        if (chunkManager.CapturedChunkCount == 0)
        {
            Debug.LogError(
                $"Surface region " +
                $"{regionDefinition.name} " +
                $"captured ZERO chunks."
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

        /*
         * The painted Tilemaps have now been
         * safely copied into SurfaceChunkData.
         *
         * Clear the original painted terrain.
         * From this point onward, terrain is
         * controlled by streaming.
         */
        chunkManager.PrepareForStreaming();

        if (overworldGrid != null &&
            cardinalMovement != null)
        {
            Vector3Int playerCell =
                overworldGrid.WorldToCell(
                    cardinalMovement
                        .transform
                        .position
                );

            RefreshSurfaceRegion(
                regionDefinition,
                chunkManager,
                playerCell
            );
        }

        UpdateCurrentGameplayRegion();
    }

    private void UpdateCurrentGameplayRegion()
    {
        if (worldManager == null)
        {
            Debug.LogError(
                "StreamingManager: WorldManager is NULL."
            );

            return;
        }

        if (overworldGrid == null)
        {
            Debug.LogError(
                "StreamingManager: OverworldGrid is NULL."
            );

            return;
        }

        if (cardinalMovement == null)
        {
            Debug.LogError(
                "StreamingManager: CardinalMovement is NULL."
            );

            return;
        }

        Vector3Int playerCell =
            overworldGrid.WorldToCell(
                cardinalMovement.transform.position
            );

        WorldCoordinate playerCoordinate =
            new WorldCoordinate(
                playerCell.x,
                currentLayerIndex,
                playerCell.y
            );

        WorldRegionDefinition playerRegion =
            worldManager.GetRegionAt(
                playerCoordinate
            );

        if (playerRegion == null)
            return;

        if (playerRegion == currentRegion)
            return;

        currentRegion =
            playerRegion;

        worldManager.SetCurrentRegion(
            playerRegion
        );

        Debug.Log(
            $"Current gameplay region changed to: " +
            $"{playerRegion.name}"
        );
    }

    private void RefreshAllSurfaceRegions()
    {
        if (cardinalMovement == null)
            return;

        if (overworldGrid == null)
            return;

        Vector3Int playerCell =
            overworldGrid.WorldToCell(
                cardinalMovement
                    .transform
                    .position
            );

        foreach (
            KeyValuePair<
                WorldRegionDefinition,
                SurfaceChunkManager
            > region in surfaceRegions)
        {
            if (region.Key == null)
                continue;

            if (region.Value == null)
                continue;

            RefreshSurfaceRegion(
                region.Key,
                region.Value,
                playerCell
            );
        }
    }

    private void RefreshSurfaceRegion(
        WorldRegionDefinition regionDefinition,
        SurfaceChunkManager chunkManager,
        Vector3Int playerCell)
    {
        if (regionDefinition == null)
            return;

        if (chunkManager == null)
            return;

        if (cardinalMovement == null)
            return;

        if (!activeChunksByRegion.TryGetValue(
                regionDefinition,
                out HashSet<ChunkCoordinate>
                    activeChunks))
        {
            activeChunks =
                new HashSet<
                    ChunkCoordinate
                >();

            activeChunksByRegion[
                regionDefinition
            ] =
                activeChunks;
        }

        ChunkCoordinate playerChunk =
            chunkManager.WorldToChunkCoordinate(
                playerCell
            );

        if (regionDefinition == currentRegion)
        {
            Debug.Log(
                $"STREAM CHECK | " +
                $"Region: {regionDefinition.name} | " +
                $"Player Chunk: {playerChunk} | " +
                $"Has Data: " +
                $"{chunkManager.HasChunkData(playerChunk)}"
            );
        }

        HashSet<ChunkCoordinate>
            desiredChunks =
                GetDesiredChunks(
                    chunkManager,
                    playerChunk
                );

        /*
         * Load chunks that have entered
         * the streaming radius.
         */
        foreach (
            ChunkCoordinate coordinate
            in desiredChunks)
        {
            if (activeChunks.Contains(
                    coordinate))
            {
                continue;
            }

            chunkManager.Load(
                coordinate
            );
        }

        /*
         * Unload chunks that have left
         * the streaming radius.
         */
        foreach (
            ChunkCoordinate coordinate
            in activeChunks)
        {
            if (desiredChunks.Contains(
                    coordinate))
            {
                continue;
            }

            chunkManager.Unload(
                coordinate
            );
        }

        activeChunksByRegion[
            regionDefinition
        ] =
            desiredChunks;
    }

    private HashSet<ChunkCoordinate>
        GetDesiredChunks(
            SurfaceChunkManager chunkManager,
            ChunkCoordinate playerChunk)
    {
        HashSet<ChunkCoordinate>
            desiredChunks =
                new HashSet<
                    ChunkCoordinate
                >();

        if (chunkManager == null)
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
                        playerChunk.x + x,
                        playerChunk.z + z
                    );

                if (!chunkManager
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

    public void SetCurrentLayer(
        int layerIndex)
    {
        currentLayerIndex =
            layerIndex;

        UpdateCurrentGameplayRegion();

        RefreshAllSurfaceRegions();
    }

    public async void LoadRegion(
        WorldRegionDefinition regionDefinition)
    {
        if (regionDefinition == null)
            return;

        if (string.IsNullOrEmpty(
                regionDefinition.SceneName))
        {
            Debug.LogError(
                $"LOAD FAILED | " +
                $"Region {regionDefinition.name} " +
                $"has no SceneName."
            );

            return;
        }

        Scene existingScene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (existingScene.isLoaded)
        {
            Debug.Log(
                $"SCENE ALREADY LOADED | " +
                $"Region: {regionDefinition.name} | " +
                $"Scene: {regionDefinition.SceneName}"
            );

            if (cardinalMovement != null &&
                overworldGrid != null)
            {
                cardinalMovement.SetWorldGrid(
                    overworldGrid
                );
            }

            UpdateCurrentGameplayRegion();

            RefreshAllSurfaceRegions();

            return;
        }

        Debug.Log(
            $"LOAD REQUEST | " +
            $"Region: {regionDefinition.name} | " +
            $"Scene: {regionDefinition.SceneName}"
        );

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                regionDefinition.SceneName,
                LoadSceneMode.Additive
            );

        if (operation == null)
        {
            Debug.LogError(
                $"LOAD FAILED TO START | " +
                $"Region: {regionDefinition.name} | " +
                $"Scene: {regionDefinition.SceneName}"
            );

            return;
        }

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        Debug.Log(
            $"LOAD COMPLETE | " +
            $"Region: {regionDefinition.name} | " +
            $"Scene: {regionDefinition.SceneName}"
        );

        Scene loadedScene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (!loadedScene.isLoaded)
        {
            Debug.LogError(
                $"SCENE NOT LOADED AFTER OPERATION | " +
                $"Region: {regionDefinition.name} | " +
                $"Scene: {regionDefinition.SceneName}"
            );

            return;
        }

        if (cardinalMovement != null &&
            overworldGrid != null)
        {
            cardinalMovement.SetWorldGrid(
                overworldGrid
            );
        }

        /*
         * SurfaceRegionController inside the
         * newly loaded scene will register its
         * SurfaceChunkManager.
         */

        UpdateCurrentGameplayRegion();

        RefreshAllSurfaceRegions();
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

        /*
         * Clear any chunks currently rendered
         * by this region before removing it.
         */
        if (surfaceRegions.TryGetValue(
                regionDefinition,
                out SurfaceChunkManager chunkManager))
        {
            if (activeChunksByRegion.TryGetValue(
                    regionDefinition,
                    out HashSet<ChunkCoordinate>
                        activeChunks))
            {
                foreach (
                    ChunkCoordinate coordinate
                    in activeChunks)
                {
                    chunkManager.Unload(
                        coordinate
                    );
                }
            }
        }

        Scene scene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (!scene.isLoaded)
            return;

        AsyncOperation operation =
            SceneManager.UnloadSceneAsync(
                scene
            );

        if (operation == null)
            return;

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        surfaceRegions.Remove(
            regionDefinition
        );

        activeChunksByRegion.Remove(
            regionDefinition
        );

        if (currentRegion ==
            regionDefinition)
        {
            currentRegion = null;

            UpdateCurrentGameplayRegion();
        }
    }
}