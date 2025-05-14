namespace HexGrid.Entity.Providers.Rotation;

using Map.Vector;

public interface IRotationProvider
{
    public HexVector Forward { get; }
    public HexVector ForRight { get; }
    public HexVector ForLeft { get; }
    public HexVector Backward { get; }
    public HexVector BackRight { get; }
    public HexVector BackLeft { get; }

    public event Action OnRotationChanged; 
    
    public bool RotateTowards(HexVector direction);
    public void Enable();
    public void Disable();
}