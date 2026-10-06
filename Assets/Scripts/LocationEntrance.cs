using UnityEngine;

public class LocationEntrance : MonoBehaviour
{
    [Header("Source")]
    [SerializeField]
    private int sourceLayerIndex;

    [SerializeField]
    private Vector2Int sourceTile;

    [Header("Destination")]
    [SerializeField]
    private string destinationLocationID;

    [SerializeField]
    private string destinationEntranceID;

    public int SourceLayerIndex => sourceLayerIndex;
    public Vector2Int SourceTile => sourceTile;

    public string DestinationLocationID => destinationLocationID;
    public string DestinationEntranceID => destinationEntranceID;

    public void Initialize(
        int sourceLayer,
        Vector2Int source,
        string locationID,
        string entranceID)
    {
        // Store source layer.

        // Store source tile.

        // Store destination location ID.

        // Store destination entrance ID.
    }
}