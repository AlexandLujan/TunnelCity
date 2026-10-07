using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("World Data")]
    [SerializeField]
    private WorldDatabase worldDatabase;

    [Header("Streaming")]
    [SerializeField]
    private StreamingManager streamingManager;

    [Header("Starting Region")]
    [SerializeField]
    private WorldRegionDefinition startingRegion;

    [Header("Surface Regions")]
    [SerializeField]
    private WorldRegionDefinition[] surfaceRegions;

    private WorldState worldState;

    private WorldRegionDefinition currentRegion;

    public WorldDatabase WorldDatabase => worldDatabase;
    public WorldState WorldState => worldState;

    public WorldRegionDefinition CurrentRegion => currentRegion;

    private void Awake()
    {
        InitializeWorldState();
    }

    private void Start()
    {
        if (streamingManager == null)
        {
            Debug.LogError(
                "WorldManager is missing a StreamingManager reference."
            );

            return;
        }

        if (startingRegion == null)
        {
            Debug.LogError(
                "WorldManager is missing a starting region."
            );

            return;
        }

        LoadSurfaceRegions();

        currentRegion = startingRegion;
    }

    private void InitializeWorldState()
    {
        worldState =
            new WorldState();
    }

    private void LoadSurfaceRegions()
    {
        Debug.Log(
            $"WORLD MANAGER SURFACE REGION COUNT: " +
            $"{surfaceRegions.Length}"
        );
        if (surfaceRegions == null ||
            surfaceRegions.Length == 0)
        {
            Debug.LogWarning(
                "WorldManager has no surface regions assigned."
            );

            streamingManager.LoadRegion(
                startingRegion
            );

            return;
        }

        foreach (
            WorldRegionDefinition region
            in surfaceRegions)
        {
            Debug.Log(
                $"WORLD MANAGER REGION ENTRY | " +
                $"{region.name} | Scene: {region.SceneName}"
            );
            if (region == null)
                continue;

            streamingManager.LoadRegion(
                region
            );
        }
    }

    public WorldRegionDefinition GetRegion(
        string regionID)
    {
        if (worldDatabase == null)
            return null;

        return worldDatabase.GetRegion(
            regionID
        );
    }

    public WorldLocationData GetLocation(
        string locationID)
    {
        if (worldDatabase == null)
            return null;

        return worldDatabase.GetLocation(
            locationID
        );
    }

    public WorldRegionDefinition GetRegionAt(
        WorldCoordinate coordinate)
    {
        if (worldDatabase == null)
            return null;

        foreach (
            WorldRegionDefinition region
            in worldDatabase.Regions)
        {
            if (region == null)
                continue;

            WorldCoordinate origin =
                region.WorldOrigin;

            Vector2Int size =
                region.RegionSizeInTiles;

            bool sameLayer =
                coordinate.layer ==
                origin.layer;

            bool withinX =
                coordinate.x >=
                origin.x &&
                coordinate.x <
                origin.x + size.x;

            bool withinZ =
                coordinate.z >=
                origin.z &&
                coordinate.z <
                origin.z + size.y;

            if (sameLayer &&
                withinX &&
                withinZ)
            {
                return region;
            }
        }

        return null;
    }

    public WorldLocationData GetLocationAt(
        WorldCoordinate coordinate)
    {
        if (worldDatabase == null)
            return null;

        foreach (
            WorldLocationData location
            in worldDatabase.Locations)
        {
            if (location == null)
                continue;

            WorldCoordinate position =
                location.WorldPosition;

            Vector2Int size =
                location.SizeInTiles;

            bool sameLayer =
                coordinate.layer ==
                position.layer;

            bool withinX =
                coordinate.x >=
                position.x &&
                coordinate.x <
                position.x + size.x;

            bool withinZ =
                coordinate.z >=
                position.z &&
                coordinate.z <
                position.z + size.y;

            if (sameLayer &&
                withinX &&
                withinZ)
            {
                return location;
            }
        }

        return null;
    }

    public bool IsValidCoordinate(
        WorldCoordinate coordinate)
    {
        return
            GetRegionAt(
                coordinate
            ) != null;
    }

    public void SetCurrentRegion(
    WorldRegionDefinition region)
    {
        if (region == null)
            return;

        currentRegion = region;
    }
}