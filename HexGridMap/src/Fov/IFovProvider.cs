namespace HexGridMap.Fov;

using Vector;

public interface IFovProvider
{
    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius);
}