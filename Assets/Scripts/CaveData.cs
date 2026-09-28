using System.Collections.Generic;

public class CaveData
{
    private int caveSeed;

    private CaveDefinition definition;

    private List<CaveLayerData> layers;

    public int CaveSeed => caveSeed;

    public CaveDefinition Definition => definition;

    public IReadOnlyList<CaveLayerData> Layers => layers;

    public CaveData(
        int caveSeed,
        CaveDefinition definition)
    {
        this.caveSeed = caveSeed;
        this.definition = definition;

        layers = new List<CaveLayerData>();
    }

    public CaveLayerData GetLayer(int layerIndex)
    {
        // Find and return the CaveLayerData
        // matching the requested logical layer.

        return null;
    }

    public bool HasLayer(int layerIndex)
    {
        // Determine whether this cave already
        // contains the requested layer.

        return false;
    }

    public void AddLayer(CaveLayerData layer)
    {
        // Validate the layer.

        // Prevent duplicate layer indices.

        // Add the layer to the cave.
    }
}