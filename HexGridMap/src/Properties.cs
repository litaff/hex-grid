namespace HexGrid.Map;

using System;

public class Properties
{
    private static Properties? instance;
    
    public static Properties Instance
    {
        get
        {
            if (instance == null)
            {
                throw new NullReferenceException("Initialize Properties before using GridMap!");
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
    
    public Properties(float cellSize, int chunkSize, float layerHeight)
    {
        this.cellSize = cellSize;
        this.chunkSize = chunkSize;
        this.layerHeight = layerHeight;
        instance = this;
    }
}