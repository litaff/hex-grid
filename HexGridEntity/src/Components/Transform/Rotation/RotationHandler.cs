namespace HexGrid.Entity.Components.Transform.Rotation;

using Map.Vector;

public class RotationHandler : IRotationHandler
{
    private readonly IRotatable rotatable;

    public bool RotationComplete { get; private set; }

    public event Action? OnRotationHandled;

    public RotationHandler(IRotatable rotatable)
    {
        this.rotatable = rotatable;
        RotationComplete = true;
    }

    public void RotateTowards(HexVector direction)
    {
        RotationComplete = false;
        var worldDirection = direction.ToWorldPosition();
        rotatable.LookTowards(worldDirection);
        OnRotationHandled?.Invoke();
        RotationComplete = true;
    }

    public void Update(double deltaTime)
    {
        
    }
}