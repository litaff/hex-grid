namespace HexGrid.Map.Vector;

using Godot;

public class HexVectorConverter(Properties properties)
{
    private float CellSize => properties.CellSize;
    private float HeightStepSize => properties.HeightStepSize;
    
    /// <summary>
    /// Returns the vertices of this hex position in world space.
    /// </summary>
    public Vector3[] GetHexVertices(HexVector hexPosition)
    {
        var worldPosition = ToWorldPosition(hexPosition);
        var vertices = new Vector3[6];
        for (var i = 0; i < 6; i++)
        {
            var angle = Mathf.DegToRad(60 * i);
            var x = worldPosition.X + CellSize * Mathf.Cos(angle);
            var z = worldPosition.Z + CellSize * Mathf.Sin(angle);
            vertices[i] = new Vector3(x, worldPosition.Y, z);
        }
        return vertices;
    }
    
    /// <summary>
    /// Converts a world position to the nearest hex position.
    /// </summary>
    public HexVector ToHexPosition(Vector3 worldPosition)
    {
        var q = 2f / 3f * worldPosition.X / CellSize;
        var r = (-1f / 3f * worldPosition.X + Mathf.Sqrt(3) / 3f * worldPosition.Z) / CellSize;
        return new HexFracVector(q, r, worldPosition.Y / HeightStepSize).Round();
    }
    
    /// <summary>
    /// Converts a hex position to a world position.
    /// </summary>
    public Vector3 ToWorldPosition(HexVector hexPosition)
    {
        var qBasis = new Vector2(3f / 2f, Mathf.Sqrt(3) / 2f);
        var rBasis = new Vector2(0, Mathf.Sqrt(3));
        
        var x = CellSize * (qBasis.X * hexPosition.Q + rBasis.X * hexPosition.R);
        var z = CellSize * (qBasis.Y * hexPosition.Q + rBasis.Y * hexPosition.R);
        return new Vector3(x, hexPosition.H * HeightStepSize, z);
    }
    
    /// <summary>
    /// Converts a hex position to a world position without the use of Properties,
    /// so this is safe to use for uninitialized properties.
    /// </summary>
    public Vector3 ToWorldPositionUnscaled(HexVector hexPosition)
    {
        var qBasis = new Vector2(3f / 2f, Mathf.Sqrt(3) / 2f);
        var rBasis = new Vector2(0, Mathf.Sqrt(3));
        
        var x = (qBasis.X * hexPosition.Q + rBasis.X * hexPosition.R);
        var z = (qBasis.Y * hexPosition.Q + rBasis.Y * hexPosition.R);
        return new Vector3(x, hexPosition.H, z);
    }
}