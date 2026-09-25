using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(
    fileName = "NewCaveTileSet",
    menuName = "Cave/Terrain Tile Set")]
public class CaveTileSet : ScriptableObject
{
    [Header("Basic Terrain")]
    public TileBase floor;
    public TileBase solidWall;

    [Header("Directional Wall Faces")]
    public TileBase northWall;
    public TileBase southWall;
    public TileBase eastWall;
    public TileBase westWall;

    [Header("Inner Corners")]
    public TileBase innerNW;
    public TileBase innerSW;
    public TileBase innerNE;
    public TileBase innerSE;

    [Header("Outer Corners")]
    public TileBase outerNW;
    public TileBase outerSW;
    public TileBase outerNE;
    public TileBase outerSE;

    [Header("Combined Straight Walls")]
    public TileBase northSouthWall;
    public TileBase eastWestWall;

    [Header("Outer Caps")]
    public TileBase capConnectedNorth;
    public TileBase capConnectedSouth;
    public TileBase capConnectedEast;
    public TileBase capConnectedWest;

    [Header("Adjacent Inner Corners")]
    public TileBase innerNW_NE;
    public TileBase innerNE_SE;
    public TileBase innerSE_SW;
    public TileBase innerSW_NW;

    [Header("Opposite Inner Corners")]
    public TileBase innerNW_SE;
    public TileBase innerNE_SW;

    [Header("Three-Way Inner Corners")]
    public TileBase innerNW_NE_SE;
    public TileBase innerNE_SE_SW;
    public TileBase innerSE_SW_NW;
    public TileBase innerSW_NW_NE;

    [Header("Four-Way Inner Corner")]
    public TileBase innerAllFour;

    [Header("Isolated Wall")]
    public TileBase singularWall;
}
