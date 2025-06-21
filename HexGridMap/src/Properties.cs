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
    public static float LayerHeight => Instance.layerHeight;

    private readonly float cellSize;
    private readonly float layerHeight;
    
    public Properties(float cellSize, float layerHeight)
    {
        this.cellSize = cellSize;
        this.layerHeight = layerHeight;
        instance = this;
    }
}