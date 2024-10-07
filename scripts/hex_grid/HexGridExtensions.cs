namespace hex_grid.scripts.hex_grid;

using System;
using System.Collections.Generic;
using Godot;
using vector;

public static class HexGridExtensions
{
    public static Vector2 QBasis => new(3f / 2f, Mathf.Sqrt(3) / 2f);
    public static Vector2 RBasis => new(0, Mathf.Sqrt(3));
    
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
    
    /// <summary>
    /// Converts a world position to the nearest hex position.
    /// </summary>
    public static CubeHexVector ToHexPosition(this Vector3 worldPosition, float cellSize)
    {
        var q = 2f / 3f * worldPosition.X / cellSize;
        var r = (-1f / 3f * worldPosition.X + Mathf.Sqrt(3) / 3f * worldPosition.Z) / cellSize;
        return new CubeHexFracVector(q, r).Round();
    }
    
    /// <summary>
    /// Converts a hex position to a world position.
    /// </summary>
    public static Vector3 ToWorldPosition(this CubeHexVector hexPosition, float cellSize)
    {
        var x = cellSize * (QBasis.X * hexPosition.Q + RBasis.X * hexPosition.R);
        var z = cellSize * (QBasis.Y * hexPosition.Q + RBasis.Y * hexPosition.R);
        return new Vector3(x, 0, z);
    }
    
    /// <summary>
    /// Returns the vertices of this hex position in world space.
    /// </summary>
    public static Vector3[] GetHexVertices(this CubeHexVector hexPosition, float cellSize)
    {
        var worldPosition = hexPosition.ToWorldPosition(cellSize);
        var vertices = new Vector3[6];
        for (var i = 0; i < 6; i++)
        {
            var angle = Mathf.DegToRad(60 * i);
            var x = worldPosition.X + cellSize * Mathf.Cos(angle);
            var z = worldPosition.Z + cellSize * Mathf.Sin(angle);
            vertices[i] = new Vector3(x, 0, z);
        }
        return vertices;
    }
    
    /// <summary>
    /// Converts a hex position to a chunk position.
    /// https://observablehq.com/@sanderevers/hexagon-tiling-of-an-hexagonal-grid#small_to_big
    /// </summary>
    public static CubeHexVector ToChunkPosition(this CubeHexVector hexPosition, int chunkSize)
    {
        var shift = 3 * chunkSize + 2;
        var area = 3 * chunkSize * chunkSize + 3 * chunkSize + 1;
        var intermediateVector = new Vector3I(
            Mathf.FloorToInt((hexPosition.R + shift * hexPosition.Q) / (float)area),
            Mathf.FloorToInt((hexPosition.S + shift * hexPosition.R) / (float)area),
            Mathf.FloorToInt((hexPosition.Q + shift * hexPosition.S) / (float)area));
        var chunkPos = new CubeHexVector(
            Mathf.FloorToInt((1 + intermediateVector.X - intermediateVector.Y) / 3f),
            Mathf.FloorToInt((1 + intermediateVector.Y - intermediateVector.Z) / 3f));
        return chunkPos;
    }

    /// <summary>
    /// Converts a chunk position to a hex position.
    /// https://observablehq.com/@sanderevers/hexmod-representation#center_of
    /// </summary>
    public static CubeHexVector FromChunkPosition(this CubeHexVector chunkPosition, int chunkSize)
    {
        return new CubeHexVector(
            (chunkSize + 1) * chunkPosition.Q - chunkSize * chunkPosition.S,
            (chunkSize + 1) * chunkPosition.R - chunkSize * chunkPosition.Q);
    }
}