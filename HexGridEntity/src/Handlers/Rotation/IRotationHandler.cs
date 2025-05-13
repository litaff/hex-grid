namespace HexGrid.Entity.Handlers.Rotation;

using Map.Vector;

public interface IRotationHandler
{
    public void RotateTowards(HexVector direction);
}