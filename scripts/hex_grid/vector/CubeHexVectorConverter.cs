namespace hex_grid.scripts.hex_grid.vector;

using Godot;

public static class CubeHexVectorConverter
{
    private static float CellSize => HexGridData.Instance.CellSize;
    private static int ChunkSize => HexGridData.Instance.ChunkSize;
    
    /// <summary>
    /// Converts a world position to the nearest hex position.
    /// </summary>
    public static CubeHexVector ToHexPosition(this Vector3 worldPosition)
    {
        var q = 2f / 3f * worldPosition.X / CellSize;
        var r = (-1f / 3f * worldPosition.X + Mathf.Sqrt(3) / 3f * worldPosition.Z) / CellSize;
        return new CubeHexFracVector(q, r).Round();
    }
    
    /// <summary>
    /// Converts a hex position to a world position.
    /// </summary>
    public static Vector3 ToWorldPosition(this CubeHexVector hexPosition)
    {
        var qBasis = new Vector2(3f / 2f, Mathf.Sqrt(3) / 2f);
        var rBasis = new Vector2(0, Mathf.Sqrt(3));
        
        var x = CellSize * (qBasis.X * hexPosition.Q + rBasis.X * hexPosition.R);
        var z = CellSize * (qBasis.Y * hexPosition.Q + rBasis.Y * hexPosition.R);
        return new Vector3(x, 0, z);
    }
    
    /// <summary>
    /// Converts a hex position to a chunk position.
    /// https://observablehq.com/@sanderevers/hexagon-tiling-of-an-hexagonal-grid#small_to_big
    /// </summary>
    public static CubeHexVector ToChunkPosition(this CubeHexVector hexPosition)
    {
        var shift = 3 * ChunkSize + 2;
        var area = 3 * ChunkSize * ChunkSize + 3 * ChunkSize + 1;
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
    public static CubeHexVector FromChunkPosition(this CubeHexVector chunkPosition)
    {
        return new CubeHexVector(
            (ChunkSize + 1) * chunkPosition.Q - ChunkSize * chunkPosition.S,
            (ChunkSize + 1) * chunkPosition.R - ChunkSize * chunkPosition.Q);
    }
    
    /// <summary>
    /// Returns a hex position relative to the chunk, which contains the hex position.
    /// https://observablehq.com/@sanderevers/hexmod-representation#rel_coords
    /// </summary>
    public static CubeHexVector RelativeToChunk(this CubeHexVector hexPosition)
    {
        var chunkPosition = hexPosition.ToChunkPosition();
        return hexPosition - chunkPosition.FromChunkPosition();
    }
}