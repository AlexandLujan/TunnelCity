using UnityEngine;

public class DrilledOpening : MonoBehaviour
{
    [Header("Source")]
    [SerializeField]
    private int sourceLayerIndex;

    [SerializeField]
    private Vector2Int sourceTile;

    [Header("Direction")]
    [SerializeField]
    private OpeningDirection direction;

    [Header("State")]
    [SerializeField]
    private bool hasExplosive;

    [SerializeField]
    private bool hasBeenBlasted;

    public int SourceLayerIndex => sourceLayerIndex;
    public Vector2Int SourceTile => sourceTile;
    public OpeningDirection Direction => direction;

    public bool HasExplosive => hasExplosive;
    public bool HasBeenBlasted => hasBeenBlasted;

    public void Initialize(
        int layerIndex,
        Vector2Int tile,
        OpeningDirection openingDirection)
    {
        // Store source layer.

        // Store source tile.

        // Store direction.

        // Reset state.
    }

    public void AttachExplosive()
    {
        // Prevent attaching after blasting.

        // Mark explosive as attached.
    }

    public void MarkBlasted()
    {
        // Remove explosive state.

        // Mark this opening as blasted.
    }
}