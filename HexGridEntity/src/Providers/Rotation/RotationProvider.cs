namespace HexGrid.Entity.Providers.Rotation;

using Map.Vector;

public class RotationProvider : IRotationProvider
{
    public HexVector Forward { get; private set; }
    public HexVector ForRight => new (-Forward.R, -Forward.S);
    public HexVector ForLeft => new (-Forward.S, -Forward.Q);
    public HexVector Backward => -Forward;
    public HexVector BackRight => -ForLeft;
    public HexVector BackLeft => -ForRight;

    public event Action? OnRotationChanged;
    
    public RotationProvider()
    {
        Forward = HexVector.North;
    }
    
    public RotationProvider(HexVector initialForward)
    {
        Forward = initialForward.Normalized();
    }
    
    public bool RotateTowards(HexVector target)
    {
        var normalizedTarget = target.Normalized();
        if (normalizedTarget == Forward) return false;
        Forward = normalizedTarget;
        OnRotationChanged?.Invoke();
        return true;
    }
}