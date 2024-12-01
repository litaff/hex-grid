namespace hex_grid_map.vector;

using System;
using System.Collections.Generic;
using Godot;

public static class CubeHexVectorExtensions
{
    private static float CellSize => HexGridData.Instance.CellSize;
    
    /// <summary>
    /// Returns the vertices of this hex position in world space.
    /// </summary>
    public static Vector3[] GetHexVertices(this CubeHexVector hexPosition)
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
    public static CubeHexVector Round(this CubeHexFracVector fraction)
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

        return new CubeHexVector(q, r);
    }
    
    /// <summary>
    /// Returns a fraction hex position between two hex positions.
    /// </summary>
    public static CubeHexFracVector Lerp(CubeHexVector a, CubeHexVector b, float t)
    {
        return new CubeHexFracVector(a.Q + (b.Q - a.Q) * t, a.R + (b.R - a.R) * t);
    }
    
    /// <summary>
    /// Returns a list of hex positions between two hexes.
    /// </summary>
    public static CubeHexVector[] LineTo(this CubeHexVector source, CubeHexVector target)
    {
        var distance = source.Distance(target);
        var lineHexes = new CubeHexVector[distance + 1];
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
    public static CubeHexVector GetNeighbor(this CubeHexVector hex, CubeHexVector direction)
    {
        return hex + direction;
    }
    
    /// <summary>
    /// Returns a list of hex positions in a ring around a center hex.
    /// The ring starts at the north direction and goes clockwise.
    /// </summary>
    public static CubeHexVector[] GetRing(this CubeHexVector center, int radius)
    {
        var directions = new[]
        {
            CubeHexVector.EastSouth, CubeHexVector.South, CubeHexVector.WestSouth, 
            CubeHexVector.WestNorth, CubeHexVector.North, CubeHexVector.EastNorth
        };
        var results = new List<CubeHexVector>();
        var hex = center.GetNeighbor(CubeHexVector.North * radius);
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
    public static CubeHexVector[] GetSpiral(this CubeHexVector center, int radius)
    {
        var results = new List<CubeHexVector> {center};
        for (var i = 1; i <= radius; i++)
        {
            results.AddRange(center.GetRing(i));
        }
        return results.ToArray();
    }
}