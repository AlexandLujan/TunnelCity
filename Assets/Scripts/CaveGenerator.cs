using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    [Header("Procedural Generation")]

    [SerializeField]
    [Min(1)]
    private int walkSteps = 130;

    [SerializeField]
    [Range(0f, 1f)]
    private float chamberChance = 0.15f;

    public bool IsGenerated { get; private set; }

    public void GenerateCave(CaveLayerData layer)
    {
        Debug.Log(
            $"GenerateCave() | Layer {layer.layerIndex} | " +
            $"{layer.width} x {layer.depth}"
        );

        if (layer == null)
        {
            Debug.LogWarning(
                "CaveGenerator: Cannot generate a null CaveLayerData.",
                this
            );

            return;
        }

        IsGenerated = false;

        // Save Unity's current random state so cave generation
        // does not permanently affect other random gameplay.
        Random.State previousRandomState = Random.state;

        Random.InitState(layer.layerSeed);

        CaveTileType solidTileType =
            GetSolidTileType(layer.caveType);

        // Initialize the entire layer.
        //
        // Outer boundary = RoughStone
        // Interior = cave-specific material
        for (int x = 0; x < layer.width; x++)
        {
            for (int z = 0; z < layer.depth; z++)
            {
                if (IsInterior(layer, x, z))
                {
                    layer.SetTile(
                        x,
                        z,
                        solidTileType
                    );
                }
                else
                {
                    layer.SetTile(
                        x,
                        z,
                        CaveTileType.RoughStone
                    );
                }
            }
        }

        // Begin the random walk in the center.
        Vector2Int position = new Vector2Int(
            layer.width / 2,
            layer.depth / 2
        );

        layer.SetTile(
            position.x,
            position.y,
            CaveTileType.Open
        );

        for (int i = 0; i < walkSteps; i++)
        {
            Vector2Int direction = GetRandomDirection();

            Vector2Int nextPosition =
                position + direction;

            if (!IsInterior(
                    layer,
                    nextPosition.x,
                    nextPosition.y))
            {
                continue;
            }

            position = nextPosition;

            layer.SetTile(
                position.x,
                position.y,
                CaveTileType.Open
            );

            if (Random.value < chamberChance)
            {
                CarveChamber(
                    layer,
                    position
                );
            }
        }

        layer.state = CaveLayerState.Generated;

        IsGenerated = true;

        // Restore Unity's random state.
        Random.state = previousRandomState;
    }

    private Vector2Int GetRandomDirection()
    {
        int direction = Random.Range(0, 4);

        switch (direction)
        {
            case 0:
                return Vector2Int.up;

            case 1:
                return Vector2Int.right;

            case 2:
                return Vector2Int.down;

            default:
                return Vector2Int.left;
        }
    }

    private void CarveChamber(
        CaveLayerData layer,
        Vector2Int center)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                int cellX = center.x + x;
                int cellZ = center.y + z;

                if (!IsInterior(
                        layer,
                        cellX,
                        cellZ))
                {
                    continue;
                }

                if (Random.value < 0.75f)
                {
                    layer.SetTile(
                        cellX,
                        cellZ,
                        CaveTileType.Open
                    );
                }
            }
        }
    }

    private bool IsInterior(
        CaveLayerData layer,
        int x,
        int z)
    {
        return x > 0 &&
               x < layer.width - 1 &&
               z > 0 &&
               z < layer.depth - 1;
    }

    private CaveTileType GetSolidTileType(
        CaveType caveType)
    {
        return caveType switch
        {
            CaveType.Dirt =>
                CaveTileType.Dirt,

            CaveType.Rock =>
                CaveTileType.Rock,

            CaveType.Crystal =>
                CaveTileType.Crystal,

            CaveType.MountainRock =>
                CaveTileType.MountainRock,

            _ =>
                CaveTileType.RoughStone
        };
    }
}