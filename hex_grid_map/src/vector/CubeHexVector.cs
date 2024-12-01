namespace hex_grid_map.vector;

using System;
using System.Text.Json.Serialization;

public struct CubeHexVector : IEquatable<CubeHexVector>
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public int S { get; private set; }
    
    public CubeHexVector(int q, int r)
    {
        Q = q;
        R = r;
        S = -q - r;
    }
    
    [JsonConstructor]
    public CubeHexVector(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
        if (Q + R + S == 0) return;
        throw new ArgumentException("[CubeHexVector] Q + R + S must be 0!");
    }
    
    public static CubeHexVector Zero => new(0, 0);

    #region Direct directions

    public static CubeHexVector North => new(0, -1);
    public static CubeHexVector EastNorth => new(1, -1);
    public static CubeHexVector EastSouth => new(1, 0);
    public static CubeHexVector South => new(0, 1);
    public static CubeHexVector WestSouth => new(-1, 1);
    public static CubeHexVector WestNorth => new(-1, 0);

    #endregion

    #region Diagonal directions

    public static CubeHexVector DiagonalEast => new(2, -1);
    public static CubeHexVector DiagonalSouthEast => new(1, 1);
    public static CubeHexVector DiagonalSouthWest => new(-1, 2);
    public static CubeHexVector DiagonalWest => new(-2, 1);
    public static CubeHexVector DiagonalNorthWest => new(-1, -1);
    public static CubeHexVector DiagonalNorthEast => new(1, -2);

    #endregion

    public int Distance(CubeHexVector target)
    {
        var direction = this - target;
        return (Math.Abs(direction.Q) + Math.Abs(direction.R) + Math.Abs(direction.S)) / 2;
    }
    
    public static CubeHexVector operator +(CubeHexVector a, CubeHexVector b)
    {
        return new CubeHexVector(a.Q + b.Q, a.R + b.R);
    }
    
    public static CubeHexVector operator -(CubeHexVector a, CubeHexVector b)
    {
        return new CubeHexVector(a.Q - b.Q, a.R - b.R);
    }
    
    public static CubeHexVector operator *(CubeHexVector a, int b)
    {
        return new CubeHexVector(a.Q * b, a.R * b);
    }
    
    public static CubeHexVector operator /(CubeHexVector a, int b)
    {
        return new CubeHexVector(a.Q / b, a.R / b);
    }
    
    public static bool operator ==(CubeHexVector a, CubeHexVector b)
    {
        return a.Q == b.Q && a.R == b.R && a.S == b.S;
    }
    
    public static bool operator !=(CubeHexVector a, CubeHexVector b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({Q}, {R}, {S})";
    }
    
    public bool Equals(CubeHexVector other)
    {
        return Q == other.Q && R == other.R && S == other.S;
    }

    public override bool Equals(object? obj)
    {
        return obj is CubeHexVector other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Q, R);
    }
}