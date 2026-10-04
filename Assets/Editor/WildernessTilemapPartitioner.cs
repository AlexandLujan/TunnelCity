using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WildernessTilemapPartitioner : EditorWindow
{
    [Header("Source Tilemaps")]
    private Tilemap groundTilemap;
    private Tilemap waterTilemap;
    private Tilemap mountainTilemap;
    private Tilemap detailTilemap;

    private const int ChunkSize = 32;

    private const int RegionChunksWide = 20;
    private const int RegionChunksTall = 18;

    private const int PartitionChunksWide = 5;
    private const int PartitionChunksTall = 6;

    private static readonly Vector2Int ChunkOrigin =
        new Vector2Int(-4, -2);

    [MenuItem("Tools/World/Partition Wilderness Tilemaps")]
    public static void ShowWindow()
    {
        GetWindow<WildernessTilemapPartitioner>(
            "Wilderness Partitioner"
        );
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Wilderness Tilemap Partitioner",
            EditorStyles.boldLabel
        );

        groundTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Ground Tilemap",
            groundTilemap,
            typeof(Tilemap),
            true
        );

        waterTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Water Tilemap",
            waterTilemap,
            typeof(Tilemap),
            true
        );

        mountainTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Mountain Tilemap",
            mountainTilemap,
            typeof(Tilemap),
            true
        );

        detailTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Detail Tilemap",
            detailTilemap,
            typeof(Tilemap),
            true
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Create Partitioned Tilemaps"))
        {
            CreatePartitions();
        }
    }

    private void CreatePartitions()
    {
        if (groundTilemap == null ||
            waterTilemap == null ||
            mountainTilemap == null ||
            detailTilemap == null)
        {
            Debug.LogError(
                "Assign all four source Tilemaps first."
            );

            return;
        }

        Grid grid = groundTilemap.GetComponentInParent<Grid>();

        if (grid == null)
        {
            Debug.LogError(
                "Ground Tilemap is not under a Grid."
            );

            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        for (int partitionZ = 0;
             partitionZ < RegionChunksTall / PartitionChunksTall;
             partitionZ++)
        {
            for (int partitionX = 0;
                 partitionX < RegionChunksWide / PartitionChunksWide;
                 partitionX++)
            {
                CreatePartition(
                    grid.transform,
                    partitionX,
                    partitionZ
                );
            }
        }

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log(
            "Finished creating Wilderness Tilemap partitions."
        );
    }

    private void CreatePartition(
        Transform gridTransform,
        int partitionX,
        int partitionZ)
    {
        string partitionName =
            $"Partition_{partitionX}_{partitionZ}";

        GameObject partitionObject =
            new GameObject(partitionName);

        Undo.RegisterCreatedObjectUndo(
            partitionObject,
            "Create Wilderness Partition"
        );

        partitionObject.transform.SetParent(
            gridTransform,
            false
        );

        Tilemap newGround =
            CreateTilemap(
                partitionObject.transform,
                "GroundTilemap"
            );

        Tilemap newWater =
            CreateTilemap(
                partitionObject.transform,
                "WaterTilemap"
            );

        Tilemap newMountain =
            CreateTilemap(
                partitionObject.transform,
                "MountainTilemap"
            );

        Tilemap newDetail =
            CreateTilemap(
                partitionObject.transform,
                "DetailTilemap"
            );

        int startChunkX =
            partitionX * PartitionChunksWide;

        int startChunkZ =
            partitionZ * PartitionChunksTall;

        int startCellX =
            ChunkOrigin.x +
            (startChunkX * ChunkSize);

        int startCellY =
            ChunkOrigin.y +
            (startChunkZ * ChunkSize);

        int width =
            PartitionChunksWide * ChunkSize;

        int height =
            PartitionChunksTall * ChunkSize;

        BoundsInt bounds = new BoundsInt(
            startCellX,
            startCellY,
            0,
            width,
            height,
            1
        );

        CopyTiles(
            groundTilemap,
            newGround,
            bounds
        );

        CopyTiles(
            waterTilemap,
            newWater,
            bounds
        );

        CopyTiles(
            mountainTilemap,
            newMountain,
            bounds
        );

        CopyTiles(
            detailTilemap,
            newDetail,
            bounds
        );
    }

    private Tilemap CreateTilemap(
        Transform parent,
        string name)
    {
        GameObject tilemapObject =
            new GameObject(name);

        Undo.RegisterCreatedObjectUndo(
            tilemapObject,
            "Create Partition Tilemap"
        );

        tilemapObject.transform.SetParent(
            parent,
            false
        );

        Tilemap tilemap =
            tilemapObject.AddComponent<Tilemap>();

        TilemapRenderer renderer =
            tilemapObject.AddComponent<TilemapRenderer>();

        renderer.mode =
            TilemapRenderer.Mode.Chunk;

        return tilemap;
    }

    private void CopyTiles(
        Tilemap source,
        Tilemap destination,
        BoundsInt bounds)
    {
        foreach (Vector3Int cellPosition
                 in bounds.allPositionsWithin)
        {
            TileBase tile =
                source.GetTile(cellPosition);

            if (tile == null)
                continue;

            destination.SetTile(
                cellPosition,
                tile
            );
        }
    }
}