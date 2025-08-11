namespace HexGrid.Entity.Components.Transform.Rotation;

using Map.Vector;

public class RotationHandler : IRotationHandler
{
    private readonly IRotatable rotatable;
    private readonly HexVectorConverter converter;

    public bool RotationComplete { get; private set; }

    public event Action? OnRotationHandled;

    public RotationHandler(IRotatable rotatable, HexVectorConverter converter)
    {
        this.converter = converter;
        this.rotatable = rotatable;
        RotationComplete = true;
    }

    public void RotateTowards(HexVector direction)
    {
        RotationComplete = false;
        var worldDirection = converter.ToWorldPosition(direction);
        rotatable.LookTowards(worldDirection);
        OnRotationHandled?.Invoke();
        RotationComplete = true;
    }

    public void Update(double deltaTime)
    {
        
    }
}