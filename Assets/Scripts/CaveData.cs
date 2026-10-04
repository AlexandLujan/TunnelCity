using System.Collections.Generic;

public class CaveData
{
    private int caveSeed;

    private CaveDefinition definition;

    private List<CaveLayerData> layers;

    public int CaveSeed => caveSeed;

    public CaveDefinition Definition => definition;

    public IReadOnlyList<CaveLayerData> Layers => layers;

    public CaveData(int caveSeed,CaveDefinition definition)
    {
        this.caveSeed = caveSeed;
        this.definition = definition;

        layers = new List<CaveLayerData>();
    }

    public CaveLayerData GetLayer(int layerIndex)
    {
        foreach (CaveLayerData layer in layers)
        {
            if (layer.layerIndex == layerIndex)
                return layer;
        }

        return null;
    }

    public bool HasLayer(int layerIndex)
    {
        foreach (CaveLayerData layer in layers)
        {
            if (layer.layerIndex == layerIndex)
                return true;
        }

        return false;
    }

    public void AddLayer(CaveLayerData layer)
    {
        if (layer == null) return;
        if (HasLayer(layer.layerIndex)) return;

        layers.Add(layer);
    }
}