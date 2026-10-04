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

    // Actual map-origin cell.
    // Partition_0_0 begins here.
    private Vector2Int mapOriginCell =
        new Vector2Int(-196, -194);

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

        mapOriginCell = EditorGUILayout.Vector2IntField(
            "Map Origin Cell",
            mapOriginCell
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Create Partitioned Tilemaps"))
        {
            CreatePartitions();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Delete Existing Partitions"))
        {
            DeleteExistingPartitions();
        }
    }

    private void CreatePartitions()
    {
        if (!HasRequiredReferences())
            return;

        Grid grid =
            groundTilemap.GetComponentInParent<Grid>();

        if (grid == null)
        {
            Debug.LogError(
                "Ground Tilemap is not under a Grid."
            );

            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        // Always remove the previous generated set first.
        DeleteExistingPartitions();

        Debug.Log(
            $"Generating Wilderness partitions from map origin " +
            $"({mapOriginCell.x}, {mapOriginCell.y})"
        );

        int partitionCountX =
            RegionChunksWide / PartitionChunksWide;

        int partitionCountZ =
            RegionChunksTall / PartitionChunksTall;

        for (int partitionZ = 0;
             partitionZ < partitionCountZ;
             partitionZ++)
        {
            for (int partitionX = 0;
                 partitionX < partitionCountX;
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
            $"Finished creating Wilderness Tilemap partitions. " +
            $"Created {partitionCountX * partitionCountZ} partitions."
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
                "GroundTilemap",
                groundTilemap
            );

        Tilemap newWater =
            CreateTilemap(
                partitionObject.transform,
                "WaterTilemap",
                waterTilemap
            );

        Tilemap newMountain =
            CreateTilemap(
                partitionObject.transform,
                "MountainTilemap",
                mountainTilemap
            );

        Tilemap newDetail =
            CreateTilemap(
                partitionObject.transform,
                "DetailTilemap",
                detailTilemap
            );

        int startChunkX =
            partitionX * PartitionChunksWide;

        int startChunkZ =
            partitionZ * PartitionChunksTall;

        int startCellX =
            mapOriginCell.x +
            (startChunkX * ChunkSize);

        int startCellY =
            mapOriginCell.y +
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

        Debug.Log(
            $"{partitionName}: " +
            $"start=({startCellX}, {startCellY}), " +
            $"size=({width}, {height})"
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
        string name,
        Tilemap source)
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

        // Match the source Tilemap's transform.
        tilemapObject.transform.localPosition =
            source.transform.localPosition;

        tilemapObject.transform.localRotation =
            source.transform.localRotation;

        tilemapObject.transform.localScale =
            source.transform.localScale;

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

    private void DeleteExistingPartitions()
    {
        if (groundTilemap == null)
        {
            Debug.LogError(
                "Assign the Ground Tilemap first."
            );

            return;
        }

        Grid grid =
            groundTilemap.GetComponentInParent<Grid>();

        if (grid == null)
        {
            Debug.LogError(
                "Ground Tilemap is not under a Grid."
            );

            return;
        }

        int deletedCount = 0;

        for (int i = grid.transform.childCount - 1;
             i >= 0;
             i--)
        {
            Transform child =
                grid.transform.GetChild(i);

            if (!child.name.StartsWith("Partition_"))
                continue;

            Undo.DestroyObjectImmediate(
                child.gameObject
            );

            deletedCount++;
        }

        Debug.Log(
            $"Deleted {deletedCount} existing Wilderness partitions."
        );
    }

    private bool HasRequiredReferences()
    {
        if (groundTilemap == null ||
            waterTilemap == null ||
            mountainTilemap == null ||
            detailTilemap == null)
        {
            Debug.LogError(
                "Assign all four source Tilemaps first."
            );

            return false;
        }

        return true;
    }
}