using System.Collections.Generic;
using UnityEngine;

public class CaveState
{
    private Dictionary<WorldCoordinate, CaveTileType> modifiedTiles;

    public IReadOnlyDictionary<WorldCoordinate, CaveTileType> ModifiedTiles
        => modifiedTiles;

    public CaveState()
    {
        modifiedTiles = new Dictionary<WorldCoordinate, CaveTileType>();
    }

    public void SetModifiedTile(
        WorldCoordinate coordinate,
        CaveTileType tileType)
    {
        modifiedTiles[coordinate] = tileType;
    }

    public bool TryGetModifiedTile(
        WorldCoordinate coordinate,
        out CaveTileType tileType)
    {
        return modifiedTiles.TryGetValue(coordinate, out tileType);
    }
}
