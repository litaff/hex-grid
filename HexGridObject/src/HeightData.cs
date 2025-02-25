namespace HexGridObject;

public readonly struct HeightData(float height, float stepHeight)
{
    public float Height { get; } = height;
    public float StepHeight { get; } = stepHeight;
}