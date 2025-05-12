namespace HexGridObject.Providers.Rotation;

using HexGridMap.Vector;

public interface IRotationProvider
{
    public CubeHexVector Forward { get; }
    public CubeHexVector ForRight { get; }
    public CubeHexVector ForLeft { get; }
    public CubeHexVector Backward { get; }
    public CubeHexVector BackRight { get; }
    public CubeHexVector BackLeft { get; }

    public event Action OnRotationChanged; 
    
    public bool RotateTowards(CubeHexVector direction);
}