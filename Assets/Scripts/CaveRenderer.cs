using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveRenderer : MonoBehaviour
{
    // References

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

    // The layer currently being displayed.
    private CaveLayerData currentLayer;

    public CaveLayerData CurrentLayer =>
        currentLayer;

    public void SetLayer(
        CaveLayerData layer)
    {
        currentLayer = layer;
    }

    // The following code is what many would say
    // "where the magic happens so PAY ATTENTION!"
    public void RenderCave()
    {
        Debug.Log(
            $"RenderCave() | Layer: {currentLayer?.layerIndex}"
        );

        if (!HasRequiredReferences())
            return;

        if (currentLayer == null)
            return;

        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        wallFaceTilemap.ClearAllTiles();

        // PASS 1:
        // Fill the grid with solid wall tops.
        for (int x = 0;
             x < currentLayer.width;
             x++)
        {
            for (int z = 0; z < currentLayer.depth; z++)
            {
                Vector3Int position = new Vector3Int(x,z,0);

                if (currentLayer.IsSolid(x, z))
                {
                    wallTilemap.SetTile(position, GetWallTile(x, z));
                }
            }
        }

        // PASS 2:
        // Carve open cells into the floor.
        for (int x = 0;
             x < currentLayer.width;
             x++)
        {
            for (int z = 0;
                 z < currentLayer.depth;
                 z++)
            {
                if (!currentLayer.IsOpen(x, z))
                    continue;

                Vector3Int position =
                    new Vector3Int(
                        x,
                        z,
                        0
                    );

                wallTilemap.SetTile(
                    position,
                    null
                );

                floorTilemap.SetTile(
                    position,
                    tileSet.floor
                );
            }
        }

        // PASS 3:
        // Add wall faces along the
        // solid/open boundary.
        for (int x = 0;
             x < currentLayer.width;
             x++)
        {
            for (int z = 0;
                 z < currentLayer.depth;
                 z++)
            {
                RenderWallFace(
                    x,
                    z
                );
            }
        }
    }

    public void ClearCave()
    {
        if (floorTilemap != null)
            floorTilemap.ClearAllTiles();

        if (wallTilemap != null)
            wallTilemap.ClearAllTiles();

        if (wallFaceTilemap != null)
            wallFaceTilemap.ClearAllTiles();
    }

    private void RenderWallFace(
        int x,
        int z)
    {
        if (!HasRequiredReferences())
            return;

        if (currentLayer == null)
            return;

        if (!currentLayer.InBounds(x, z))
            return;

        Vector3Int position =
            new Vector3Int(
                x,
                z,
                0
            );

        wallFaceTilemap.SetTile(
            position,
            null
        );

        if (!currentLayer.IsSolid(x, z))
            return;

        TileBase selectedTile =
            SelectPerimeterTile(
                x,
                z
            );

        if (selectedTile != null)
        {
            wallFaceTilemap.SetTile(
                position,
                selectedTile
            );

            Debug.Log(
                $"WallFace ({x}, {z}) = " +
                $"{wallFaceTilemap.GetTile(position)?.name}"
                );
        }
    }

    private TileBase SelectPerimeterTile(
        int x,
        int z)
    {
        // Cardinal neighbors.
        //
        // True = this side of the solid
        // wall borders open floor.
        bool openN =
            currentLayer.IsOpen(
                x,
                z + 1
            );

        bool openE =
            currentLayer.IsOpen(
                x + 1,
                z
            );

        bool openS =
            currentLayer.IsOpen(
                x,
                z - 1
            );

        bool openW =
            currentLayer.IsOpen(
                x - 1,
                z
            );

        // Diagonal neighbors.
        bool openNW =
            currentLayer.IsOpen(
                x - 1,
                z + 1
            );

        bool openNE =
            currentLayer.IsOpen(
                x + 1,
                z + 1
            );

        bool openSW =
            currentLayer.IsOpen(
                x - 1,
                z - 1
            );

        bool openSE =
            currentLayer.IsOpen(
                x + 1,
                z - 1
            );

        const int CARDINAL_MASK =
            1 | 2 | 4 | 8;

        const int DIAGONAL_MASK =
            16 | 32 | 64 | 128;

        int neighborMask = 0;

        if (openN)
            neighborMask |= 1;

        if (openE)
            neighborMask |= 2;

        if (openS)
            neighborMask |= 4;

        if (openW)
            neighborMask |= 8;

        if (openNW)
            neighborMask |= 16;

        if (openNE)
            neighborMask |= 32;

        if (openSW)
            neighborMask |= 64;

        if (openSE)
            neighborMask |= 128;

        int cardinalMask =
            CARDINAL_MASK &
            neighborMask;

        int diagonalMask =
            DIAGONAL_MASK &
            neighborMask;

        int relevantCorners = 0;

        switch (cardinalMask)
        {
            case 0:
                return SelectInnerCornerTile(
                    openNW,
                    openNE,
                    openSW,
                    openSE
                );

            case 1:
                relevantCorners =
                    diagonalMask &
                    (16 | 32);

                switch (relevantCorners)
                {
                    case 0:
                        return tileSet.northWall;

                    case 16:
                        return tileSet.northNW;

                    case 32:
                        return tileSet.northNE;

                    case 16 | 32:
                        return tileSet.northNWNE;
                }

                break;

            case 2:
                relevantCorners =
                    diagonalMask &
                    (32 | 128);

                switch (relevantCorners)
                {
                    case 0:
                        return tileSet.eastWall;

                    case 32:
                        return tileSet.eastNE;

                    case 128:
                        return tileSet.eastSE;

                    case 32 | 128:
                        return tileSet.eastNESE;
                }

                break;

            case 4:
                relevantCorners =
                    diagonalMask &
                    (64 | 128);

                switch (relevantCorners)
                {
                    case 0:
                        return tileSet.southWall;

                    case 64:
                        return tileSet.southSW;

                    case 128:
                        return tileSet.southSE;

                    case 64 | 128:
                        return tileSet.southSWSE;
                }

                break;

            case 8:
                relevantCorners =
                    diagonalMask &
                    (16 | 64);

                switch (relevantCorners)
                {
                    case 0:
                        return tileSet.westWall;

                    case 16:
                        return tileSet.westNW;

                    case 64:
                        return tileSet.westSW;

                    case 16 | 64:
                        return tileSet.westNWSW;
                }

                break;

            case 1 | 2:
                return tileSet.outerSW;

            case 1 | 8:
                return tileSet.outerSE;

            case 4 | 2:
                return tileSet.outerNW;

            case 4 | 8:
                return tileSet.outerNE;

            case 1 | 4:
                return tileSet.northSouthWall;

            case 2 | 8:
                return tileSet.eastWestWall;

            case 2 | 4 | 8:
                return tileSet.capConnectedNorth;

            case 1 | 4 | 8:
                return tileSet.capConnectedEast;

            case 1 | 2 | 8:
                return tileSet.capConnectedSouth;

            case 1 | 2 | 4:
                return tileSet.capConnectedWest;

            case 1 | 2 | 4 | 8:
                return tileSet.singularWall;
        }

        return null;
    }

    private TileBase SelectInnerCornerTile(
        bool openNW,
        bool openNE,
        bool openSW,
        bool openSE)
    {
        int diagonalMask = 0;

        if (openNW)
            diagonalMask |= 1;

        if (openNE)
            diagonalMask |= 2;

        if (openSE)
            diagonalMask |= 4;

        if (openSW)
            diagonalMask |= 8;

        switch (diagonalMask)
        {
            case 0:
                return null;

            case 1:
                return tileSet.innerSE;

            case 2:
                return tileSet.innerSW;

            case 4:
                return tileSet.innerNW;

            case 8:
                return tileSet.innerNE;

            case 1 | 2:
                return tileSet.innerNW_NE;

            case 2 | 4:
                return tileSet.innerNE_SE;

            case 4 | 8:
                return tileSet.innerSE_SW;

            case 8 | 1:
                return tileSet.innerSW_NW;

            case 1 | 4:
                return tileSet.innerNW_SE;

            case 2 | 8:
                return tileSet.innerNE_SW;

            case 1 | 2 | 4:
                return tileSet.innerNW_NE_SE;

            case 2 | 4 | 8:
                return tileSet.innerNE_SE_SW;

            case 4 | 8 | 1:
                return tileSet.innerSE_SW_NW;

            case 8 | 1 | 2:
                return tileSet.innerSW_NW_NE;

            case 1 | 2 | 4 | 8:
                return tileSet.innerAllFour;
        }

        return null;
    }

    private void RenderCell(
        int x,
        int z)
    {
        if (!HasRequiredReferences())
            return;

        if (currentLayer == null)
            return;

        if (!currentLayer.InBounds(x, z))
            return;

        Vector3Int cellPosition =
            new Vector3Int(
                x,
                z,
                0
            );

        floorTilemap.SetTile(
            cellPosition,
            null
        );

        wallTilemap.SetTile(
            cellPosition,
            null
        );

        wallFaceTilemap.SetTile(
            cellPosition,
            null
        );

        if (currentLayer.IsOpen(x, z))
        {
            floorTilemap.SetTile(
                cellPosition,
                tileSet.floor
            );
        }
        else
        {
            wallTilemap.SetTile(cellPosition, GetWallTile(x, z));
        }

        RenderWallFace(
            x,
            z
        );
    }

    private void RefreshAround(
        int centerX,
        int centerZ)
    {
        // A mined cell can change the
        // cardinal and diagonal neighbor
        // patterns of every cell in this
        // 3x3 area.

        for (int x = centerX - 1;
             x <= centerX + 1;
             x++)
        {
            for (int z = centerZ - 1;
                 z <= centerZ + 1;
                 z++)
            {
                if (!currentLayer.InBounds(
                        x,
                        z))
                {
                    continue;
                }

                RenderCell(
                    x,
                    z
                );
            }
        }
    }

    public bool MineWall(
        int x,
        int z)
    {
        if (!HasRequiredReferences())
            return false;

        if (currentLayer == null)
            return false;

        if (!currentLayer.IsMineable(x, z))
            return false;

        currentLayer.SetTile(
            x,
            z,
            CaveTileType.Open
        );

        RefreshAround(
            x,
            z
        );

        return true;
    }

    public bool MineWall(
        Vector3Int cell)
    {
        return MineWall(
            cell.x,
            cell.y
        );
    }

    public Vector3Int WorldToCell(
        Vector3 worldPosition)
    {
        return floorTilemap.WorldToCell(
            worldPosition
        );
    }

    private bool HasRequiredReferences()
    {
        if (floorTilemap == null ||
            wallTilemap == null ||
            wallFaceTilemap == null ||
            tileSet == null)
        {
            Debug.LogWarning(
                "CaveRenderer: Missing assignments.",
                this
            );

            return false;
        }

        return true;
    }

    private TileBase GetWallTile(int x, int z)
    {
        CaveTileType tileType = currentLayer.GetTile(x, z);

        if (tileType == CaveTileType.RoughStone)
            return tileSet.roughStone;

        return tileSet.solidWall;
    }

    private void DiagnoseCell(
        int x,
        int z)
    {
        if (!HasRequiredReferences())
            return;

        if (currentLayer == null)
            return;

        if (!currentLayer.InBounds(x, z))
            return;

        string State(
            int cellX,
            int cellZ)
        {
            CaveTileType tileType =
                currentLayer.GetTile(
                    cellX,
                    cellZ
                );

            if (tileType ==
                CaveTileType.Open)
            {
                return "O";
            }

            if (tileType ==
                CaveTileType.RoughStone)
            {
                return "B";
            }

            return "S";
        }

        Vector3Int position =
            new Vector3Int(
                x,
                z,
                0
            );

        TileBase floor =
            floorTilemap.GetTile(
                position
            );

        TileBase wall =
            wallTilemap.GetTile(
                position
            );

        TileBase face =
            wallFaceTilemap.GetTile(
                position
            );

        Debug.Log(
            $"DIAGNOSTIC ({x}, {z})\n" +
            $"Cell: {State(x, z)}\n" +
            $"Tile Type: {currentLayer.GetTile(x, z)}\n" +
            $"Layer: {currentLayer.layerIndex}\n" +
            $"Neighbors:\n" +
            $"{State(x - 1, z + 1)} {State(x, z + 1)} {State(x + 1, z + 1)}\n" +
            $"{State(x - 1, z)} [{State(x, z)}] {State(x + 1, z)}\n" +
            $"{State(x - 1, z - 1)} {State(x, z - 1)} {State(x + 1, z - 1)}\n" +
            $"FloorTilemap: {(floor != null ? floor.name : "EMPTY")}\n" +
            $"WallTilemap: {(wall != null ? wall.name : "EMPTY")}\n" +
            $"WallFaceTilemap: {(face != null ? face.name : "EMPTY")}",
            this
        );
    }
}