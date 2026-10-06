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
        foreach (WorldRegionDefinition region in regions)
        {
            if (region.RegionID == regionID)
                return region;
        }

        return null;
    }

    public WorldLocationData GetLocation(string locationID)
    {
        foreach (WorldLocationData location in locations)
        {
            if (location.LocationID == locationID)
                return location;
        }

        return null;
    }
}