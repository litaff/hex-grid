namespace HexGridObject.Handlers.Rotation;

using HexGridMap.Vector;

public class InstantRotationHandler : IRotationHandler
{
    private readonly IRotatable rotatable;
    
    public InstantRotationHandler(IRotatable rotatable)
    {
        this.rotatable = rotatable;
    }
    
    public void RotateTowards(CubeHexVector direction)
    {
        var worldDirection = direction.ToWorldPosition();
        rotatable.LookTowards(worldDirection);
    }
}