using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CaveGenerator))]
public class CaveRenderer : MonoBehaviour
{
    // References

    [Header("Cave Data")]
    [SerializeField]
    private CaveGenerator caveGenerator;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private Tilemap wallFaceTilemap;

    [Header("Terrain Tileset")]
    [SerializeField]
    private CaveTileSet tileSet;

    // Initialization
    private void Awake()
    {
        if (caveGenerator == null) caveGenerator = GetComponent<CaveGenerator>();
    }

    private void Start()
    {
        StartCoroutine(InitializeRenderer());
    }

    private IEnumerator InitializeRenderer()
    {
        yield return null;

        if (HasRequiredReferences() && caveGenerator.IsGenerated) RenderCave();
    }

    // The following code is what many would say "where the magic happens so PAY ATTENTION!
    public void RenderCave()
    {
        if (!HasRequiredReferences() || !caveGenerator.IsGenerated) return;

        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        wallFaceTilemap.ClearAllTiles();

        // PASS 1: Fill the grid with solid wall tops.
        for (int x = 0; x < CaveGenerator.Width; x++)
        {
            for (int y = 0; y < CaveGenerator.Height; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                wallTilemap.SetTile(position, tileSet.solidWall);
            }
        }

        // PASS 2: Carve open cells into the floor.
        for (int x = 0; x < CaveGenerator.Width; x++)
        {
            for (int y = 0; y < CaveGenerator.Height; y++)
            {
                if (!caveGenerator.IsOpen(x, y)) continue;

                Vector3Int position = new Vector3Int(x, y, 0);

                wallTilemap.SetTile(position, null);
                floorTilemap.SetTile(position, tileSet.floor);
            }
        }

        // PASS 3: Add wall faces along the solid/open boundary.
        for (int x = 0; x < CaveGenerator.Width; x++)
        {
            for (int y = 0; y < CaveGenerator.Height; y++)
            {
                RenderWallFace(x, y);
            }
        }
    }

    private void RenderWallFace(int x, int y)
    {
        if (!HasRequiredReferences() || !caveGenerator.InBounds(x, y)) return;

        Vector3Int position = new Vector3Int(x, y, 0);

        wallFaceTilemap.SetTile(position, null);

        if (!caveGenerator.IsSolid(x, y)) return;

        TileBase selectedTile = SelectPerimeterTile(x, y);

        if (selectedTile != null) wallFaceTilemap.SetTile(position, selectedTile);
    }

    private TileBase SelectPerimeterTile(int x, int y)
    {
        // Cardinal neighbors.
        // True = this side of the solid wall borders open floor.
        bool openN = caveGenerator.IsOpen(x, y + 1);
        bool openE = caveGenerator.IsOpen(x + 1, y);
        bool openS = caveGenerator.IsOpen(x, y - 1);
        bool openW = caveGenerator.IsOpen(x - 1, y);

        // Diagonal neighbors.
        bool openNW = caveGenerator.IsOpen(x - 1, y + 1);
        bool openNE = caveGenerator.IsOpen(x + 1, y + 1);
        bool openSW = caveGenerator.IsOpen(x - 1, y - 1);
        bool openSE = caveGenerator.IsOpen(x + 1, y - 1);

        if (openS && openE &&
            !openN && !openW &&
            tileSet.outerNW != null)
        {
            return tileSet.outerNW;
        }

        if (openS && openW &&
            !openN && !openE &&
            tileSet.outerNE != null)
        {
            return tileSet.outerNE;
        }

        if (openN && openE &&
            !openS && !openW &&
            tileSet.outerSW != null)
        {
            return tileSet.outerSW;
        }

        if (openN && openW &&
            !openS && !openE &&
            tileSet.outerSE != null)
        {
            return tileSet.outerSE;
        }

        if (!openN && !openE && !openS && !openW)
        {
            if (openNW && tileSet.innerSE != null)
                return tileSet.innerSE;

            if (openNE && tileSet.innerSW != null)
                return tileSet.innerSW;

            if (openSW && tileSet.innerNE != null)
                return tileSet.innerNE;

            if (openSE && tileSet.innerNW != null)
                return tileSet.innerNW;
        }

        if (openS && tileSet.southWall != null) return tileSet.southWall;

        if (openN && tileSet.northWall != null) return tileSet.northWall;

        if (openE && tileSet.eastWall != null) return tileSet.eastWall;

        if (openW && tileSet.westWall != null) return tileSet.westWall;

        return null;
    }

    public void RenderCell(int x, int y)
    {
        if (!HasRequiredReferences()) return;
        if (!caveGenerator.InBounds(x, y)) return;

        Vector3Int position = new Vector3Int(x, y, 0);

        floorTilemap.SetTile(position, null);
        wallTilemap.SetTile(position, null);
        wallFaceTilemap.SetTile(position, null);

        if (caveGenerator.IsOpen(x, y))
            floorTilemap.SetTile(position, tileSet.floor);
        else
            wallTilemap.SetTile(position, tileSet.solidWall);

        RenderWallFace(x, y);
    }

    private void RefreshAround(int centerX, int centerY)
    {
        // A mined cell can change the cardinal
        // and diagonal neighbor patterns of
        // every cell in this 3x3 area.

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = centerX + dx;
                int y = centerY + dy;

                if (!caveGenerator.InBounds(x, y)) continue;

                RenderCell(x, y);
            }
        }
    }

    public bool MineWall(int x, int y)
    {
        if (!HasRequiredReferences()) return false;
        if (!caveGenerator.MineCell(x, y)) return false;
        RefreshAround(x, y);
        return true;
    }

    public void RegenerateCave()
    {
        if (!HasRequiredReferences()) return;
        caveGenerator.GenerateCave();
        RenderCave();
    }

    private bool HasRequiredReferences()
    {
        if (caveGenerator == null ||
            floorTilemap == null ||
            wallTilemap == null ||
            wallFaceTilemap == null ||
            tileSet == null)
        {
            Debug.LogWarning("CaveRenderer: Missing assignments", this);
            return false;
        }
        return true;
    }

    private void DiagnoseCell(int x, int y)
    {
        if (!HasRequiredReferences() || !caveGenerator.InBounds(x, y))
            return;

        // S = solid, O = open.
        string State(int cx, int cy)
        {
            return caveGenerator.IsSolid(cx, cy) ? "S" : "O";
        }

        Vector3Int position = new Vector3Int(x, y, 0);

        TileBase floor = floorTilemap.GetTile(position);
        TileBase wall = wallTilemap.GetTile(position);
        TileBase face = wallFaceTilemap.GetTile(position);

        Debug.Log(
            $"DIAGNOSTIC ({x}, {y})\n" +
            $"Cell: {State(x, y)}\n" +
            $"Neighbors:\n" +
            $"{State(x - 1, y + 1)} {State(x, y + 1)} {State(x + 1, y + 1)}\n" +
            $"{State(x - 1, y)} [{State(x, y)}] {State(x + 1, y)}\n" +
            $"{State(x - 1, y - 1)} {State(x, y - 1)} {State(x + 1, y - 1)}\n" +
            $"FloorTilemap: {(floor != null ? floor.name : "EMPTY")}\n" +
            $"WallTilemap: {(wall != null ? wall.name : "EMPTY")}\n" +
            $"WallFaceTilemap: {(face != null ? face.name : "EMPTY")}",
            this
        );
    }
}
