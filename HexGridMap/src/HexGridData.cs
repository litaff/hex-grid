namespace HexGridMap;

using System;

public class HexGridData
{
    public static HexGridData Instance
    {
        get
        {
            if (instance == null)
            {
                throw new NullReferenceException("Initialize HexGridData before using HexGridMap!");
            }
            return instance;
        }
    }

    public static bool Exists => instance != null;

    private static HexGridData? instance;

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