namespace HexGrid.Map;

public class Properties(float cellSize, float heightStepSize)
{
    public float CellSize { get; } = cellSize;
    public float HeightStepSize { get; } = heightStepSize;
}