using System.Collections.Generic;

public class WorldState
{
    private Dictionary<WorldCoordinate, CaveTileType> modifiedTiles;

    //private List<EnemyState> enemyStates;
    //private List<ItemState> itemStates;
    //private List<ObjectState> objectStates;

    public IReadOnlyDictionary<WorldCoordinate, CaveTileType> ModifiedTiles => modifiedTiles;

    //public IReadOnlyList<EnemyState> EnemyStates => enemyStates;
    //public IReadOnlyList<ItemState> ItemStates => itemStates;
    //public IReadOnlyList<ObjectState> ObjectStates => objectStates;

    public WorldState()
    {
        modifiedTiles = new Dictionary<WorldCoordinate, CaveTileType>();

        //enemyStates = new List<EnemyState>();
        //itemStates = new List<ItemState>();
        //objectStates = new List<ObjectState>();
    }

    public void SetModifiedTile(
        WorldCoordinate coordinate,
        CaveTileType tileType)
    {
        // Store or update a modified tile.
    }

    public bool TryGetModifiedTile(
        WorldCoordinate coordinate,
        out CaveTileType tileType)
    {
        // Try to retrieve a player-modified tile.

        tileType = default;
        return false;
    }
}