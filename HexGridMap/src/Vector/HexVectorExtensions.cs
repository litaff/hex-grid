namespace HexGrid.Map.Vector;

using System;
using System.Collections.Generic;
using Godot;

public static class HexVectorExtensions
{
    private static float CellSize => Properties.CellSize;
    
    /// <summary>
    /// Returns the vertices of this hex position in world space.
    /// </summary>
    public static Vector3[] GetHexVertices(this HexVector hexPosition)
    {
        var worldPosition = hexPosition.ToWorldPosition();
        var vertices = new Vector3[6];
        for (var i = 0; i < 6; i++)
        {
            var angle = Mathf.DegToRad(60 * i);
            var x = worldPosition.X + CellSize * Mathf.Cos(angle);
            var z = worldPosition.Z + CellSize * Mathf.Sin(angle);
            vertices[i] = new Vector3(x, 0, z);
        }
        return vertices;
    }
    
    /// <summary>
    /// Rounds a fraction hex position to the nearest hex position.
    /// </summary>
    public static HexVector Round(this HexFracVector fraction)
    {
        var q = (int)Math.Round(fraction.Q);
        var r = (int)Math.Round(fraction.R);
        var s = (int)Math.Round(fraction.S);
        
        var qDiff = Math.Abs(q - fraction.Q);
        var rDiff = Math.Abs(r - fraction.R);
        var sDiff = Math.Abs(s - fraction.S);
        
        if (qDiff > rDiff && qDiff > sDiff)
        {
            q = -r - s;
        }
        else if (rDiff > sDiff)
        {
            r = -q - s;
        }

        return new HexVector(q, r);
    }
    
    /// <summary>
    /// Returns a fraction hex position between two hex positions.
    /// </summary>
    public static HexFracVector Lerp(HexVector a, HexVector b, float t)
    {
        return new HexFracVector(a.Q + (b.Q - a.Q) * t, a.R + (b.R - a.R) * t);
    }
    
    /// <summary>
    /// Returns a list of hex positions between two hexes.
    /// </summary>
    public static HexVector[] LineTo(this HexVector source, HexVector target)
    {
        var distance = source.Distance(target);
        var lineHexes = new HexVector[distance + 1];
        for (var i = 0; i <= distance; i++)
        {
            var vector = Lerp(source, target, 1f / distance * i).Round();
            lineHexes[i] = vector;
        }
        return lineHexes;
    }
    
    /// <summary>
    /// Returns a position of a neighbor hex in a given direction.
    /// </summary>
    public static HexVector GetNeighbor(this HexVector hex, HexVector direction)
    {
        return hex + direction;
    }
    
    /// <summary>
    /// Returns a list of hex positions in a ring around a center hex.
    /// The ring starts at the north direction and goes clockwise.
    /// </summary>
    public static HexVector[] GetRing(this HexVector center, int radius)
    {
        var directions = new[]
        {
            HexVector.EastSouth, HexVector.South, HexVector.WestSouth, 
            HexVector.WestNorth, HexVector.North, HexVector.EastNorth
        };
        var results = new List<HexVector>();
        var hex = center.GetNeighbor(HexVector.North * radius);
        for (var i = 0; i < 6; i++)
        {
            for (var j = 0; j < radius; j++)
            {
                results.Add(hex);
                hex = hex.GetNeighbor(directions[i]);
            }
        }
        return results.ToArray();
    }
    
    /// <summary>
    /// Returns a list of hex positions in a spiral around a center hex.
    /// </summary>
    public static HexVector[] GetSpiral(this HexVector center, int radius)
    {
        var results = new List<HexVector> {center};
        for (var i = 1; i <= radius; i++)
        {
            results.AddRange(center.GetRing(i));
        }
        return results.ToArray();
    }
}