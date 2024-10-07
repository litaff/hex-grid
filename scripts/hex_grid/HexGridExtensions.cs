namespace hex_grid.scripts.hex_grid;

using System;
using vector;

public static class HexGridExtensions
{
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
}