using System;
using UnityEngine;

[Serializable]
public struct ChunkCoordinate : IEquatable<ChunkCoordinate>
{
    public int x;
    public int z;

    public ChunkCoordinate(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public bool Equals(ChunkCoordinate other)
    {
        return x == other.x
            && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is ChunkCoordinate other
            && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public static bool operator ==(
        ChunkCoordinate left,
        ChunkCoordinate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        ChunkCoordinate left,
        ChunkCoordinate right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"({x}, {z})";
    }
}