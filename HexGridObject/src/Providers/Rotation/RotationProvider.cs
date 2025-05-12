namespace HexGridObject.Providers.Rotation;

using HexGridMap.Vector;

public class RotationProvider : IRotationProvider
{
    public CubeHexVector Forward { get; private set; }
    public CubeHexVector ForRight => new (-Forward.R, -Forward.S);
    public CubeHexVector ForLeft => new (-Forward.S, -Forward.Q);
    public CubeHexVector Backward => -Forward;
    public CubeHexVector BackRight => -ForLeft;
    public CubeHexVector BackLeft => -ForRight;

    public event Action? OnRotationChanged; 
    
    public RotationProvider(CubeHexVector initialForward)
    {
        Forward = initialForward.Normalized();
    }
    
    public bool RotateTowards(CubeHexVector target)
    {
        var normalizedTarget = target.Normalized();
        if (normalizedTarget == Forward) return false;
        Forward = normalizedTarget;
        OnRotationChanged?.Invoke();
        return true;
    }
}