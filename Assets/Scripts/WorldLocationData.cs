using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWorldLocationData",
    menuName = "World/Location Data"
)]
public class WorldLocationData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string locationID;

    [SerializeField]
    private string displayName;

    [SerializeField]
    private LocationType locationType;

    [Header("World Placement")]
    [SerializeField]
    private WorldCoordinate worldPosition;

    [SerializeField]
    private Vector2Int sizeInTiles;

    public string LocationID => locationID;
    public string DisplayName => displayName;
    public LocationType LocationType => locationType;

    public WorldCoordinate WorldPosition => worldPosition;
    public Vector2Int SizeInTiles => sizeInTiles;
}
