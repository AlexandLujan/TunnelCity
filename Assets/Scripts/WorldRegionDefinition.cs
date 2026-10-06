using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWorldRegionDefinition",
    menuName = "World/Region Definition"
)]
public class WorldRegionDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string regionID;

    [SerializeField]
    private string displayName;

    [SerializeField]
    private WorldType worldType;

    [Header("World Placement")]
    [SerializeField]
    private WorldCoordinate worldOrigin;

    [Header("Chunk Settings")]
    [SerializeField]
    private Vector2Int regionSizeInChunks;

    [SerializeField]
    private int chunkSize;

    [Header("Scene")]
    [SerializeField]
    private string sceneName;

    public string RegionID => regionID;
    public string DisplayName => displayName;
    public WorldType WorldType => worldType;

    public WorldCoordinate WorldOrigin => worldOrigin;

    public Vector2Int RegionSizeInChunks => regionSizeInChunks;
    public int ChunkSize => chunkSize;

    public string SceneName => sceneName;

    public Vector2Int RegionSizeInTiles => regionSizeInChunks * chunkSize;
}
