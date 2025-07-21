namespace HexGrid.Entity;

public readonly struct HeightData(float height, int stepRange)
{
    public float Height { get; } = height;
    public int StepRange { get; } = stepRange;
}