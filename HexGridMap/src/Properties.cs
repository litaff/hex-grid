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
    public static float HeightStep => Instance.heightStep;

    private readonly float cellSize;
    private readonly float heightStep;
    
    public Properties(float cellSize, float heightStep)
    {
        this.cellSize = cellSize;
        this.heightStep = heightStep;
        instance = this;
    }
}