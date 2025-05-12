namespace HexGridObject.Handlers.Rotation;

using HexGridMap.Vector;

public interface IRotationHandler
{
    public void RotateTowards(CubeHexVector direction);
}