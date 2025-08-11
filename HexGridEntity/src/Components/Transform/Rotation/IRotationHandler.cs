namespace HexGrid.Entity.Components.Transform.Rotation;

using Map.Vector;

public interface IRotationHandler
{
    public bool RotationComplete { get; }
    
    public event Action OnRotationHandled;

    public void RotateTowards(HexVector direction);
    public void Update(double deltaTime);
}