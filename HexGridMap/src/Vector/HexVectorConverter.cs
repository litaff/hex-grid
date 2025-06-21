namespace HexGrid.Map.Vector;

using Godot;

public static class HexVectorConverter
{
    private static float CellSize => Properties.CellSize;
    
    /// <summary>
    /// Converts a world position to the nearest hex position.
    /// </summary>
    public static HexVector ToHexPosition(this Vector3 worldPosition)
    {
        var q = 2f / 3f * worldPosition.X / CellSize;
        var r = (-1f / 3f * worldPosition.X + Mathf.Sqrt(3) / 3f * worldPosition.Z) / CellSize;
        return new HexFracVector(q, r).Round();
    }
    
    /// <summary>
    /// Converts a hex position to a world position.
    /// </summary>
    public static Vector3 ToWorldPosition(this HexVector hexPosition)
    {
        var qBasis = new Vector2(3f / 2f, Mathf.Sqrt(3) / 2f);
        var rBasis = new Vector2(0, Mathf.Sqrt(3));
        
        var x = CellSize * (qBasis.X * hexPosition.Q + rBasis.X * hexPosition.R);
        var z = CellSize * (qBasis.Y * hexPosition.Q + rBasis.Y * hexPosition.R);
        return new Vector3(x, 0, z);
    }
    
    /// <summary>
    /// Converts a hex position to a world position without the use of Properties,
    /// so this is safe to use for uninitialized properties.
    /// </summary>
    public static Vector3 ToWorldPositionUnscaled(this HexVector hexPosition)
    {
        var qBasis = new Vector2(3f / 2f, Mathf.Sqrt(3) / 2f);
        var rBasis = new Vector2(0, Mathf.Sqrt(3));
        
        var x = (qBasis.X * hexPosition.Q + rBasis.X * hexPosition.R);
        var z = (qBasis.Y * hexPosition.Q + rBasis.Y * hexPosition.R);
        return new Vector3(x, 0, z);
    }
}