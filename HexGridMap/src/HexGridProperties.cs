namespace HexGridMap;

using System;

public class HexGridProperties
{
    private static HexGridProperties? instance;
    
    public static HexGridProperties Instance
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
    public static float CellSize => Instance.cellSize;
    public static int ChunkSize => Instance.chunkSize;
    public static float LayerHeight => Instance.layerHeight;

    private readonly float cellSize;
    private readonly int chunkSize;
    private readonly float layerHeight;
    
    public HexGridProperties(float cellSize, int chunkSize, float layerHeight)
    {
        this.cellSize = cellSize;
        this.chunkSize = chunkSize;
        this.layerHeight = layerHeight;
        instance = this;
    }
}