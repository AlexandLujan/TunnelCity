using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("World Data")]
    [SerializeField]
    private WorldDatabase worldDatabase;

    private WorldState worldState;

    public WorldDatabase WorldDatabase => worldDatabase;
    public WorldState WorldState => worldState;

    private void Awake()
    {
        InitializeWorldState();
    }

    private void InitializeWorldState()
    {
        // Create or load runtime world state.
    }

    public WorldRegionDefinition GetRegion(string regionID)
    {
        // Query WorldDatabase.

        return null;
    }

    public WorldLocationData GetLocation(string locationID)
    {
        // Query WorldDatabase.

        return null;
    }

    public WorldRegionDefinition GetRegionAt(WorldCoordinate coordinate)
    {
        // Determine which region contains this coordinate.

        return null;
    }

    public WorldLocationData GetLocationAt(WorldCoordinate coordinate)
    {
        // Determine whether a named authored location
        // occupies this coordinate.

        return null;
    }

    public bool IsValidCoordinate(WorldCoordinate coordinate)
    {
        // Determine whether this coordinate belongs
        // to a valid region of the game world.

        return false;
    }
}