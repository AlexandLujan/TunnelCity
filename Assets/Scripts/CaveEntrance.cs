using UnityEngine;

public class CaveEntrance : MonoBehaviour
{
    [SerializeField]
    private int sourceLayerIndex;

    [SerializeField]
    private int targetLayerIndex;

    [SerializeField]
    private Vector2Int sourceTile;

    [SerializeField]
    private Vector2Int targetTile;

    private bool isOpen;

    public int SourceLayerIndex => sourceLayerIndex;
    public int TargetLayerIndex => targetLayerIndex;
    public Vector2Int SourceTile => sourceTile;
    public Vector2Int TargetTile => targetTile;

    public bool IsOpen => isOpen;

    public void Open() { isOpen = true; }

    public void Initialize(
        int sourceLayer,
        int targetLayer,
        Vector2Int source,
        Vector2Int target)
    {
        // Store source layer.

        // Store target layer.

        // Store source tile.

        // Store target tile.
    }

    public int GetOtherLayer(int currentLayerIndex)
    {
        // Return the opposite layer.

        return 0;
    }

    public Vector2Int GetDestinationTile(int currentLayerIndex)
    {
        // Return the corresponding destination tile
        // based on which side the player entered from.

        return default;
    }
}
