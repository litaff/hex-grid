namespace HexGrid.Map.Vector;

using System;
using System.Text.Json.Serialization;

public struct HexVector : IEquatable<HexVector>
{
    public int Q { get; private set; }
    public int R { get; private set; }
    [JsonIgnore]
    public int S { get; private set; }
    
    [JsonConstructor]
    public HexVector(int q, int r)
    {
        Q = q;
        R = r;
        S = -q - r;
    }

    public static Dictionary<Direction, HexVector> Directions => new()
    {
        {Direction.Forward, Forward},
        {Direction.ForRight, ForRight},
        {Direction.BackRight, BackRight},
        {Direction.Backward, Backward},
        {Direction.BackLeft, BackLeft},
        {Direction.ForLeft, ForLeft}
    };
    
    public static HexVector Zero => new(0, 0);

    #region Direct directions

    public static HexVector Forward => new(0, -1);
    public static HexVector ForRight => new(1, -1);
    public static HexVector BackRight => new(1, 0);
    public static HexVector Backward => new(0, 1);
    public static HexVector BackLeft => new(-1, 1);
    public static HexVector ForLeft => new(-1, 0);

    #endregion

    #region Diagonal directions

    public static HexVector DiagonalRight => new(2, -1);
    public static HexVector DiagonalBackRight => new(1, 1);
    public static HexVector DiagonalBackLeft => new(-1, 2);
    public static HexVector DiagonalLeft => new(-2, 1);
    public static HexVector DiagonalForLeft => new(-1, -1);
    public static HexVector DiagonalForRight => new(1, -2);

    #endregion

    public int Distance(HexVector target)
    {
        var direction = this - target;
        return (Math.Abs(direction.Q) + Math.Abs(direction.R) + Math.Abs(direction.S)) / 2;
    }

    /// <summary>
    /// Returns the closest normal approximation of this vector. If the vector is perfectly diagonal,
    /// returns a normal vector clockwise.
    /// </summary>
    public HexVector Normalized()
    {
        if (Q == 0 || R == 0 || S == 0)
        {
            return new HexVector(Math.Sign(Q), Math.Sign(R));
        }
        // Vector needs an approximation.
        var q = Math.Abs(Q);
        var r = Math.Abs(R);
        var s = Math.Abs(S);
        if (q > r && q > s)
        {
            return r > s ? new HexVector(Math.Sign(Q), Math.Sign(R)) : new HexVector(Math.Sign(Q), 0);
        }

        if (r > q && r > s)
        {
            return s > q ? new HexVector(0, Math.Sign(R)) : new HexVector(Math.Sign(Q), Math.Sign(R));
        }

        if (s > q && s > r)
        {
            return q > r ? new HexVector(Math.Sign(Q), 0) : new HexVector(0, Math.Sign(R));
        }

        // Catch case: Likely will never happen.
        return this;
    }

    public HexVector DirectionTo(HexVector target)
    {
        return target - this;
    }
    
    public static HexVector operator +(HexVector a, HexVector b)
    {
        return new HexVector(a.Q + b.Q, a.R + b.R);
    }
    
    public static HexVector operator -(HexVector a, HexVector b)
    {
        return new HexVector(a.Q - b.Q, a.R - b.R);
    }

    public static HexVector operator -(HexVector a)
    {
        return new HexVector(-a.Q, -a.R);
    }
    
    public static HexVector operator *(HexVector a, int b)
    {
        return new HexVector(a.Q * b, a.R * b);
    }
    
    public static HexVector operator /(HexVector a, int b)
    {
        return new HexVector(a.Q / b, a.R / b);
    }
    
    public static bool operator ==(HexVector a, HexVector b)
    {
        return a.Q == b.Q && a.R == b.R && a.S == b.S;
    }
    
    public static bool operator !=(HexVector a, HexVector b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return $"({Q}, {R}, {S})";
    }
    
    public bool Equals(HexVector other)
    {
        return Q == other.Q && R == other.R && S == other.S;
    }

    public override bool Equals(object? obj)
    {
        return obj is HexVector other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Q, R);
    }
}