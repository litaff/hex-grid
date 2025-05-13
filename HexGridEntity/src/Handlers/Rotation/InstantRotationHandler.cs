namespace HexGrid.Entity.Handlers.Rotation;

using Map.Vector;

public class InstantRotationHandler : IRotationHandler
{
    private readonly IRotatable rotatable;
    
    public InstantRotationHandler(IRotatable rotatable)
    {
        this.rotatable = rotatable;
    }
    
    public void RotateTowards(HexVector direction)
    {
        var worldDirection = direction.ToWorldPosition();
        rotatable.LookTowards(worldDirection);
    }
}