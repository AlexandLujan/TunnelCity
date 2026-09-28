using System;
using UnityEngine;

[Serializable]
public struct WorldCoordinate
{
    public int x;
    public int layer;
    public int z;

    public WorldCoordinate(int x, int layer, int z)
    {
        this.x = x;
        this.layer = layer;
        this.z = z;
    }

    public bool Equals(WorldCoordinate other)
    {
        return x == other.x
            && layer == other.layer
            && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is WorldCoordinate other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, layer, z);
    }

    public static bool operator ==(
        WorldCoordinate left,
        WorldCoordinate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        WorldCoordinate left,
        WorldCoordinate right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"({x}, Layer {layer}, {z})";
    }
}
