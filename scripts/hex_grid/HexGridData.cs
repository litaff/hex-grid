namespace hex_grid.scripts.hex_grid;

using System;

public class HexGridData
{
    public static HexGridData Instance
    {
        get
        {
            if (instance == null)
            {
                throw new NullReferenceException("HexGridData not yet initialized!");
            }
            return instance;
        }
    }

    private static HexGridData instance;

    public float CellSize { get; }
    public int ChunkSize { get; }
    public float LayerHeight { get; }
    
    public HexGridData(float cellSize, int chunkSize, float layerHeight)
    {
        CellSize = cellSize;
        ChunkSize = chunkSize;
        LayerHeight = layerHeight;
        instance = this;
    }
}