using UnityEngine;

public class CaveManager : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private CaveGenerator caveGenerator;

    [SerializeField]
    private CaveRenderer caveRenderer;

    [Header("Cave Definition")]

    [SerializeField]
    private CaveDefinition caveDefinition;

    [Header("Initial Cave")]

    [SerializeField]
    private int caveSeed = 12345;

    [SerializeField]
    private int initialLayerIndex = 0;

    private CaveData currentCave;
    private CaveLayerData currentLayer;

    public CaveData CurrentCave => currentCave;
    public CaveLayerData CurrentLayer => currentLayer;

    private void Awake()
    {
        if (caveGenerator == null)
            caveGenerator = GetComponent<CaveGenerator>();

        if (caveRenderer == null)
            caveRenderer = GetComponent<CaveRenderer>();
    }

    private void Start()
    {
        Debug.Log("CaveManager.Start()");
        InitializeCave();
    }

    public void InitializeCave()
    {
        Debug.Log(
            $"InitializeCave() | " +
            $"Definition: {caveDefinition?.name} | " +
            $"Size: {caveDefinition?.WidthInTiles} x {caveDefinition?.DepthInTiles}"
        );

        if (!HasRequiredReferences())
            return;

        currentCave = new CaveData(
            caveSeed,
            caveDefinition
        );

        int initialLayerSeed =
            GenerateLayerSeed(initialLayerIndex);

        CaveLayerData initialLayer =
            new CaveLayerData(
                initialLayerIndex,
                initialLayerSeed,
                caveDefinition.WidthInTiles,
                caveDefinition.DepthInTiles,
                caveDefinition.CaveType
            );

        currentCave.AddLayer(initialLayer);

        ActivateLayer(initialLayerIndex);
    }

    public CaveLayerData GetLayer(int layerIndex)
    {
        if (currentCave == null)
            return null;

        return currentCave.GetLayer(layerIndex);
    }

    public bool HasLayer(int layerIndex)
    {
        return currentCave != null &&
               currentCave.HasLayer(layerIndex);
    }

    public CaveLayerData CreateLayer(
        int layerIndex)
    {
        if (currentCave == null ||
            caveDefinition == null)
        {
            return null;
        }

        CaveLayerData existing =
            currentCave.GetLayer(layerIndex);

        if (existing != null)
            return existing;

        int layerSeed =
            GenerateLayerSeed(layerIndex);

        CaveLayerData newLayer =
            new CaveLayerData(
                layerIndex,
                layerSeed,
                caveDefinition.WidthInTiles,
                caveDefinition.DepthInTiles,
                caveDefinition.CaveType
            );

        currentCave.AddLayer(newLayer);

        return newLayer;
    }

    public bool ActivateLayer(int layerIndex)
    {
        if (!HasRequiredReferences())
            return false;

        if (currentCave == null)
            return false;

        CaveLayerData targetLayer =
            currentCave.GetLayer(layerIndex);

        if (targetLayer == null)
            return false;

        Debug.Log(
            $"Activating layer {targetLayer.layerIndex} | " +
            $"State before activation: {targetLayer.state}"
        );

        // The previous layer remains stored,
        // but is no longer active.
        if (currentLayer != null &&
            currentLayer != targetLayer)
        {
            currentLayer.state =
                CaveLayerState.Cached;
        }

        // Only generate layers that have never
        // previously been generated.
        if (targetLayer.state ==
            CaveLayerState.DataOnly)
        {
            caveGenerator.GenerateCave(
                targetLayer
            );
        }

        currentLayer = targetLayer;

        currentLayer.state =
            CaveLayerState.Active;

        caveRenderer.SetLayer(
            currentLayer
        );

        caveRenderer.RenderCave();

        return true;
    }

    public bool ChangeLayer(int targetLayerIndex)
    {
        if (currentCave == null)
            return false;

        CaveLayerData targetLayer =
            currentCave.GetLayer(
                targetLayerIndex
            );

        if (targetLayer == null)
        {
            targetLayer =
                CreateLayer(targetLayerIndex);

            if (targetLayer == null)
                return false;
        }

        return ActivateLayer(
            targetLayerIndex
        );
    }

    private int GenerateLayerSeed(
        int layerIndex)
    {
        unchecked
        {
            return caveSeed +
                   (layerIndex * 73856093);
        }
    }

    private bool HasRequiredReferences()
    {
        if (caveGenerator == null ||
            caveRenderer == null ||
            caveDefinition == null)
        {
            Debug.LogWarning(
                "CaveManager: Missing assignments.",
                this
            );

            return false;
        }

        return true;
    }

    public bool BlastEntrance(int targetLayerIndex)
    {
        return ChangeLayer(targetLayerIndex);
    }

    public bool BlastEntrance(OpeningDirection direction)
    {
        if (currentLayer == null)
            return false;

        int targetLayerIndex = currentLayer.layerIndex;

        switch (direction)
        {
            case OpeningDirection.Down:
                targetLayerIndex--;

                break;

            case OpeningDirection.Up:
                targetLayerIndex++;

                break;

            default:
                return false;
        }

        return ChangeLayer(targetLayerIndex);
    }

    public bool BlastEntrance(DrilledOpening entrance)
    {
        if (entrance.SourceLayerIndex != currentLayer.layerIndex) return false;
        if (entrance == null) return false;
        if (currentLayer == null) return false;
        if (entrance.HasBeenBlasted) return false;

        int targetLayerIndex = entrance.SourceLayerIndex;

        switch (entrance.Direction)
        {
            case OpeningDirection.Down:
                targetLayerIndex--;
                break;

            case OpeningDirection.Up:
                targetLayerIndex++;
                break;

            default:
                return false;
        }

        entrance.MarkBlasted();

        return ChangeLayer(targetLayerIndex);
    }
}

