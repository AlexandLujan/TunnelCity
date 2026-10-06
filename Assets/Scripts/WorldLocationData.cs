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

    [Header("World Classification")]
    [SerializeField]
    private WorldType worldType;

    [SerializeField]
    private OverworldType overworldType;

    [SerializeField]
    private UnderworldType underworldType;

    [SerializeField]
    private InteriorType interiorType;

    [Header("World Placement")]
    [SerializeField]
    private WorldCoordinate worldPosition;

    [SerializeField]
    private Vector2Int sizeInTiles;

    public string LocationID => locationID;
    public string DisplayName => displayName;

    public WorldType WorldType => worldType;

    public OverworldType OverworldType => overworldType;
    public UnderworldType UnderworldType => underworldType;
    public InteriorType InteriorType => interiorType;

    public WorldCoordinate WorldPosition => worldPosition;
    public Vector2Int SizeInTiles => sizeInTiles;
}