using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WorldDatabase",
    menuName = "World/World Database"
)]
public class WorldDatabase : ScriptableObject
{
    [Header("Regions")]
    [SerializeField]
    private List<WorldRegionDefinition> regions;

    [Header("Locations")]
    [SerializeField]
    private List<WorldLocationData> locations;

    public IReadOnlyList<WorldRegionDefinition> Regions => regions;
    public IReadOnlyList<WorldLocationData> Locations => locations;

    public WorldRegionDefinition GetRegion(string regionID)
    {
        // Find and return the matching region.

        return null;
    }

    public WorldLocationData GetLocation(string locationID)
    {
        // Find and return the matching named location.

        return null;
    }
}