using UnityEngine;

public class DrillSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CaveManager caveManager;

    [SerializeField]
    private Camera worldCamera;

    [Header("Drill Settings")]
    [SerializeField]
    private OpeningDirection drillDirection = OpeningDirection.Down;

    public void TryDrill()
    {
        // Determine target tile from player/input.

        // Validate that drilling is allowed here.

        // Prevent duplicate openings.

        // Create DrilledOpening.

        // Initialize it with:
        // current layer
        // source tile
        // drill direction
    }

    private bool CanDrill(Vector2Int tile)
    {
        // Validate tile.

        // Validate current layer.

        // Validate destination direction.

        // Check whether an opening already exists.

        return false;
    }
}