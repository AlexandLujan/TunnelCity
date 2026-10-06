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
        sourceLayerIndex = layerIndex;
        sourceTile = tile;
        direction = openingDirection;

        hasExplosive = false;
        hasBeenBlasted = false;
    }

    public void AttachExplosive()
    {
        if (hasBeenBlasted)
            return;

        if (hasExplosive)
            return;

        hasExplosive = true;
    }

    public void MarkBlasted()
    {
        hasExplosive = false;
        hasBeenBlasted = true;
    }
}