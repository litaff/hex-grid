namespace HexGrid.Map.Vector;

using Godot;

public static class HexVectorConverter
{
    private static float CellSize => Properties.CellSize;
    private static int ChunkSize => Properties.ChunkSize;
    
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
    /// <summary>
    /// Converts a hex position to a chunk position.
    /// https://observablehq.com/@sanderevers/hexagon-tiling-of-an-hexagonal-grid#small_to_big
    /// </summary>
    public static HexVector ToChunkPosition(this HexVector hexPosition)
    {
        var shift = 3 * ChunkSize + 2;
        var area = 3 * ChunkSize * ChunkSize + 3 * ChunkSize + 1;
        var intermediateVector = new Vector3I(
            Mathf.FloorToInt((hexPosition.R + shift * hexPosition.Q) / (float)area),
            Mathf.FloorToInt((hexPosition.S + shift * hexPosition.R) / (float)area),
            Mathf.FloorToInt((hexPosition.Q + shift * hexPosition.S) / (float)area));
        var chunkPos = new HexVector(
            Mathf.FloorToInt((1 + intermediateVector.X - intermediateVector.Y) / 3f),
            Mathf.FloorToInt((1 + intermediateVector.Y - intermediateVector.Z) / 3f));
        return chunkPos;
    }

    /// <summary>
    /// Converts a chunk position to a hex position.
    /// https://observablehq.com/@sanderevers/hexmod-representation#center_of
    /// </summary>
    public static HexVector FromChunkPosition(this HexVector chunkPosition)
    {
        return new HexVector(
            (ChunkSize + 1) * chunkPosition.Q - ChunkSize * chunkPosition.S,
            (ChunkSize + 1) * chunkPosition.R - ChunkSize * chunkPosition.Q);
    }
    
    /// <summary>
    /// Returns a hex position relative to the chunk, which contains the hex position.
    /// https://observablehq.com/@sanderevers/hexmod-representation#rel_coords
    /// </summary>
    public static HexVector RelativeToChunk(this HexVector hexPosition)
    {
        var chunkPosition = hexPosition.ToChunkPosition();
        return hexPosition - chunkPosition.FromChunkPosition();
    }
}