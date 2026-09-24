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
}
