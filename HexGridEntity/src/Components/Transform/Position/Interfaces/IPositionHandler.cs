namespace HexGrid.Entity.Components.Transform.Position.Interfaces;

using Map.Vector;

public interface IPositionHandler
{
    public bool MoveComplete { get; }
    
    public event Action OnPositionReady; 
    
    public void MoveTo(HexVector position);
    public void Update(double deltaTime);
    public void Enable();
    public void Disable();
}