using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public const int Width = 11;
    public const int Height = 11;

    [Header("Procedural Generation")]

    [SerializeField]
    [Min(1)]
    private int walkSteps = 130;

    [SerializeField]
    [Range(0f, 1f)]
    private float chamberChance = 0.15f;

    [SerializeField]
    private bool generateOnStart = true;

    // Cave Data
    private bool[,] solid;

    public bool IsGenerated { get; private set; }

    // Initialize
    private void Awake()
    {
        solid = new bool[Width, Height];
    }

    private void Start()
    {
        if (generateOnStart) GenerateCave();
    }

    // Procedural Generation
    public void GenerateCave()
    {
        IsGenerated = false;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                solid[x, y] = true;
            }
        }

        Vector2Int position = new Vector2Int(Width / 2, Height / 2);
        solid[position.x, position.y] = false;

        for (int i = 0; i < walkSteps; i++)
        {
            Vector2Int Direction = GetRandomDirection();

            Vector2Int nextPosition = position + Direction;

            if (!IsInterior(nextPosition.x, nextPosition.y)) continue;
            position = nextPosition;

            solid[position.x, position.y] = false;

            if (Random.value < chamberChance) CarveChamber(position);
            IsGenerated = true;

            // Debug.Log("Cave Generation Completed.");
        }
    }

    private Vector2Int GetRandomDirection()
    {
        int direction = Random.Range(0, 4);

        switch(direction)
        {
            case 0:
                return Vector2Int.up;
            case 1:
                return Vector2Int.right;
            case 2:
                return Vector2Int.down;
            default:
                return Vector2Int.left;
        }
    }

    private void CarveChamber(Vector2Int center)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int cellX = center.x + x;
                int cellY = center.y + y;

                if (!IsInterior(cellX, cellY)) continue;

                if (Random.value < 0.75f) solid[cellX, cellY] = false;
            }
        }
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 
            && x < Width 
            && y >= 0 
            && y < Height;
    }

    public bool IsInterior(int x, int y)
    {
        return x > 0
            && x < Width - 1
            && y > 0
            && y < Height - 1;
    }

    public bool IsSolid(int x, int y)
    {
        if (!InBounds(x, y)) return true;

        return solid[x, y];
    }

    public bool IsOpen(int x, int y) { return InBounds(x, y) && !solid[x, y]; }

    public bool MineCell(int x, int y)
    {
        if (!IsGenerated || !IsInterior(x, y)) return false;
        if (!solid[x, y]) return false;

        solid[x, y] = false;
        return true;
    }
}
