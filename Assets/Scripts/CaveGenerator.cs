using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public bool IsGenerated { get; private set; }

    public void GenerateCave(CaveLayerData layer, CaveDefinition definition)
    {
        if (layer == null)
        {
            Debug.LogWarning(
                "CaveGenerator: Cannot generate a null CaveLayerData.",
                this
            );
            return;
        }

        if (definition == null)
        {
            Debug.LogWarning(
                "CaveGenerator: Cannot generate without a CaveDefinition.",
                this
            );
            return;
        }

        Debug.Log(
            $"GenerateCave() | " +
            $"Layer {layer.layerIndex} | " +
            $"{layer.width} x {layer.depth} | " +
            $"Seed: {layer.layerSeed} | " +
            $"Density: {definition.CarveDensity}"
        );

        IsGenerated = false;
        Random.State previousRandomState = Random.state;

        try
        {
            Random.InitState(layer.layerSeed);
            int boundaryThickness = Mathf.Max(1, definition.BoundaryThickness);
            CaveTileType solidTileType = GetSolidTileType(layer.caveType);
            InitializeLayer(layer, solidTileType, boundaryThickness);

            int interiorWidth = layer.width - (boundaryThickness * 2);
            int interiorDepth = layer.depth - (boundaryThickness * 2);

            if (interiorWidth <= 0 || interiorDepth <= 0)
            {
                Debug.LogWarning(
                    "CaveGenerator: Boundary thickness leaves no interior space.",
                    this
                );

                return;
            }

            int interiorCellCount = interiorWidth * interiorDepth;
            int targetOpenCells = Mathf.Max(1, Mathf.RoundToInt(interiorCellCount * definition.CarveDensity));
            Vector2Int position = new Vector2Int(layer.width / 2, layer.depth / 2);

            int carvedCells = CarveTile(
                    layer,
                    position.x,
                    position.y,
                    boundaryThickness
                );

            int generationAttempts = 0;

            // Prevent a pathological random walk from
            // running forever if it repeatedly revisits
            // already-open cells.
            int maxGenerationAttempts = Mathf.Max(1000, targetOpenCells * 20);

            while (
                carvedCells < targetOpenCells &&
                generationAttempts <
                maxGenerationAttempts)
            {
                generationAttempts++;
                Vector2Int direction = GetRandomDirection();
                Vector2Int nextPosition = position + direction;

                if (!IsInterior(
                        layer,
                        nextPosition.x,
                        nextPosition.y,
                        boundaryThickness))
                {
                    continue;
                }

                position = nextPosition;
                carvedCells += CarveTile(
                    layer,
                    position.x,
                    position.y,
                    boundaryThickness
                    );

                if (Random.value < definition.ChamberChance)
                    carvedCells += CarveChamber(layer, position, boundaryThickness);
            }

            layer.state = CaveLayerState.Generated;
            IsGenerated = true;

            Debug.Log(
                $"Cave generated | " +
                $"Open cells: {carvedCells} / " +
                $"{targetOpenCells} target | " +
                $"Attempts: {generationAttempts}"
            );

            if (carvedCells < targetOpenCells)
            {
                Debug.LogWarning(
                    $"CaveGenerator reached its generation attempt limit. " +
                    $"Generated {carvedCells} / {targetOpenCells} target cells.",
                    this
                );
            }
        }
        finally
        {
            // Cave generation should not influence
            // random numbers used elsewhere in gameplay.
            Random.state = previousRandomState;
        }
    }

    private void InitializeLayer(CaveLayerData layer, CaveTileType solidTileType, int boundaryThickness)
    {
        for (int x = 0; x < layer.width; x++)
        {
            for (int z = 0; z < layer.depth; z++)
            {
                if (IsInterior(layer, x, z, boundaryThickness))
                    layer.SetTile(x, z, solidTileType);
                else
                    layer.SetTile(x, z, CaveTileType.RoughStone);
            }
        }
    }

    private int CarveTile(CaveLayerData layer, int x, int z, int boundaryThickness)
    {
        if (!IsInterior(layer, x, z, boundaryThickness)) return 0;
        if (layer.IsOpen(x, z)) return 0;
        layer.SetTile(x, z, CaveTileType.Open); return 1;
    }

    private int CarveChamber(
        CaveLayerData layer,
        Vector2Int center,
        int boundaryThickness)
    {
        int carvedCells = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                int cellX = center.x + x;
                int cellZ = center.y + z;

                if (Random.value >= 0.75f) continue;

                carvedCells += CarveTile(layer, cellX, cellZ, boundaryThickness);
            }
        }
        return carvedCells;
    }

    private Vector2Int GetRandomDirection()
    {
        int direction = Random.Range(0, 4);

        switch (direction)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.right;
            case 2: return Vector2Int.down;
            default: return Vector2Int.left;
        }
    }

    private bool IsInterior(CaveLayerData layer, int x, int z, int boundaryThickness)
    {
        return
            x >= boundaryThickness &&
            x < layer.width - boundaryThickness &&
            z >= boundaryThickness &&
            z < layer.depth - boundaryThickness;
    }

    private CaveTileType GetSolidTileType(CaveType caveType)
    {
        return caveType switch
        {
            CaveType.Dirt => CaveTileType.Dirt,
            CaveType.Rock => CaveTileType.Rock,
            CaveType.Crystal => CaveTileType.Crystal,
            CaveType.MountainRock => CaveTileType.MountainRock,
            _ => CaveTileType.RoughStone
        };
    }
}