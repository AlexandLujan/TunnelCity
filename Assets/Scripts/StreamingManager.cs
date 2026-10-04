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

    private WorldRegionDefinition currentRegion;

    private ChunkCoordinate currentChunk;

    private HashSet<ChunkCoordinate> activeChunks;

    private SurfaceChunkManager surfaceChunkManager;

    private int currentLayerIndex;

    private void Awake()
    {
        activeChunks =
            new HashSet<ChunkCoordinate>();
    }

    private void Update()
    {
        if (surfaceChunkManager == null)
            return;

        if (cardinalMovement == null)
            return;

        ChunkCoordinate playerChunk =
            surfaceChunkManager.WorldToChunkCoordinate(
                cardinalMovement.transform.position
            );

        UpdateCurrentChunk(playerChunk);
    }

    public void SetSurfaceChunkManager(
        SurfaceChunkManager chunkManager)
    {
        surfaceChunkManager = chunkManager;

        activeChunks.Clear();

        if (surfaceChunkManager == null)
            return;

        surfaceChunkManager.PrepareForStreaming();

        if (cardinalMovement == null)
            return;

        ChunkCoordinate startingChunk =
            surfaceChunkManager.WorldToChunkCoordinate(
                cardinalMovement.transform.position
            );

        InitializeChunks(startingChunk);
    }

    public void SetCurrentLayer(
        int layerIndex)
    {
        currentLayerIndex = layerIndex;
    }

    public async void LoadRegion(
        WorldRegionDefinition regionDefinition)
    {
        if (regionDefinition == null)
            return;

        if (string.IsNullOrEmpty(
            regionDefinition.SceneName))
            return;

        Scene existingScene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (existingScene.isLoaded)
        {
            currentRegion = regionDefinition;

            AssignRegionGrid(existingScene);

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

        currentRegion = regionDefinition;

        AssignRegionGrid(loadedScene);
    }

    public async void UnloadRegion(
        WorldRegionDefinition regionDefinition)
    {
        if (regionDefinition == null)
            return;

        if (string.IsNullOrEmpty(
            regionDefinition.SceneName))
            return;

        Scene scene =
            SceneManager.GetSceneByName(
                regionDefinition.SceneName
            );

        if (!scene.isLoaded)
            return;

        AsyncOperation operation =
            SceneManager.UnloadSceneAsync(scene);

        if (operation == null)
            return;

        while (!operation.isDone)
            await Task.Yield();

        if (currentRegion == regionDefinition)
        {
            currentRegion = null;

            activeChunks.Clear();

            surfaceChunkManager = null;

            if (cardinalMovement != null)
            {
                cardinalMovement.SetWorldGrid(null);
            }
        }
    }

    public void InitializeChunks(
        ChunkCoordinate startingChunk)
    {
        currentChunk = startingChunk;

        RefreshChunks();
    }

    public void UpdateCurrentChunk(
        ChunkCoordinate coordinate)
    {
        if (coordinate == currentChunk)
            return;

        currentChunk = coordinate;

        RefreshChunks();
    }

    private void RefreshChunks()
    {
        if (surfaceChunkManager == null)
            return;

        HashSet<ChunkCoordinate> desiredChunks =
            GetDesiredChunks();

        foreach (ChunkCoordinate coordinate
                 in desiredChunks)
        {
            if (!activeChunks.Contains(coordinate))
            {
                surfaceChunkManager.Load(
                    coordinate
                );
            }
        }

        foreach (ChunkCoordinate coordinate
                 in activeChunks)
        {
            if (!desiredChunks.Contains(coordinate))
            {
                surfaceChunkManager.Unload(
                    coordinate
                );
            }
        }

        activeChunks = desiredChunks;
    }

    private HashSet<ChunkCoordinate>
        GetDesiredChunks()
    {
        HashSet<ChunkCoordinate> desiredChunks =
            new HashSet<ChunkCoordinate>();

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
                    .HasChunkData(coordinate))
                {
                    continue;
                }

                desiredChunks.Add(coordinate);
            }
        }

        return desiredChunks;
    }

    private void AssignRegionGrid(
        Scene scene)
    {
        if (!scene.isLoaded)
            return;

        Grid regionGrid =
            GetGridFromScene(scene);

        if (regionGrid == null)
            return;

        if (cardinalMovement == null)
            return;

        cardinalMovement.SetWorldGrid(
            regionGrid
        );
    }

    private Grid GetGridFromScene(
        Scene scene)
    {
        GameObject[] rootObjects =
            scene.GetRootGameObjects();

        foreach (GameObject rootObject
                 in rootObjects)
        {
            Grid grid =
                rootObject
                    .GetComponentInChildren<Grid>();

            if (grid != null)
                return grid;
        }

        return null;
    }
}