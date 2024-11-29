namespace hex_grid.scripts.hex_grid.grid_object;

public readonly struct HeightData(float height, float stepHeight)
{
    public float Height { get; } = height;
    public float StepHeight { get; } = stepHeight;
}