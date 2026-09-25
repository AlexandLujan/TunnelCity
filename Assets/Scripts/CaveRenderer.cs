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

        int cardinalMask = 0;

        if (openN) cardinalMask |= 1;
        if (openE) cardinalMask |= 2;
        if (openS) cardinalMask |= 4;
        if (openW) cardinalMask |= 8;

        switch (cardinalMask)
        {
            case 0: return SelectInnerCornerTile(openNW, openNE, openSW, openSE);
            case 1: return tileSet.northWall;
            case 2: return tileSet.eastWall;
            case 4: return tileSet.southWall;
            case 8: return tileSet.westWall;
            case 1 | 2: return tileSet.outerSW;
            case 1 | 8: return tileSet.outerSE;
            case 4 | 2: return tileSet.outerNW;
            case 4 | 8: return tileSet.outerNE;
            case 1 | 4: return tileSet.northSouthWall;
            case 2 | 8: return tileSet.eastWestWall;
            case 2 | 4 | 8: return tileSet.capConnectedNorth;
            case 1 | 4 | 8: return tileSet.capConnectedEast;
            case 1 | 2 | 8: return tileSet.capConnectedSouth;
            case 1 | 2 | 4: return tileSet.capConnectedWest;
            case 1 | 2 | 4 | 8: return tileSet.singularWall;
        }

        return null;
    }

    private TileBase SelectInnerCornerTile(bool openNW, bool openNE, bool openSW, bool openSE)
    {
        int diagonalMask = 0;

        if (openNW) diagonalMask |= 1;
        if (openNE) diagonalMask |= 2;
        if (openSE) diagonalMask |= 4;
        if (openSW) diagonalMask |= 8;

        switch (diagonalMask)
        {
            case 0: return null;
            case 1: return tileSet.innerSE;
            case 2: return tileSet.innerSW;
            case 4: return tileSet.innerNW;
            case 8: return tileSet.innerNE;
            case 1 | 2: return tileSet.innerNW_NE;
            case 2 | 4: return tileSet.innerNE_SE;
            case 4 | 8: return tileSet.innerSE_SW;
            case 8 | 1: return tileSet.innerSW_NW;
            case 1 | 4: return tileSet.innerNW_SE;
            case 2 | 8: return tileSet.innerNE_SW;
            case 1 | 2 | 4: return tileSet.innerNW_NE_SE;
            case 2 | 4 | 8: return tileSet.innerNE_SE_SW;
            case 4 | 8 | 1: return tileSet.innerSE_SW_NW;
            case 8 | 1 | 2: return tileSet.innerSW_NW_NE;
            case 1 | 2 | 4 | 8: return tileSet.innerAllFour;
        }

        return null;
    }

    private void RenderCell(int x, int y)
    {
        if (!HasRequiredReferences()) return;
        if (!caveGenerator.InBounds(x, y)) return;

        Vector3Int cellPosition = new Vector3Int(x, y, 0);

        floorTilemap.SetTile(cellPosition, null);
        wallTilemap.SetTile(cellPosition, null);
        wallFaceTilemap.SetTile(cellPosition, null);

        if (caveGenerator.IsOpen(x, y))
            floorTilemap.SetTile(cellPosition, tileSet.floor);
        else
            wallTilemap.SetTile(cellPosition, tileSet.solidWall);

        RenderWallFace(x, y);
    }

    private void RefreshAround(int centerX, int centerY)
    {
        // A mined cell can change the cardinal
        // and diagonal neighbor patterns of
        // every cell in this 3x3 area.

        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (!caveGenerator.InBounds(x, y)) continue;
                RenderCell(x, y);
            }
        }
    }

    public bool MineWall(int x, int y)
    {
        if (!HasRequiredReferences() || !caveGenerator.IsGenerated) return false;

        bool mined = caveGenerator.MineCell(x, y);
        if (!mined) return false;

        RefreshAround(x, y);
        return true;
    }

    public bool MineWall(Vector3Int cell) { return MineWall(cell.x, cell.y); }

    public Vector3Int WorldToCell(Vector3 worldPosition) { return floorTilemap.WorldToCell(worldPosition); }

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
