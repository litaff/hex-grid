namespace HexGrid.Map.Fov;

using Vector;

public interface IFovProvider
{
    public HexVector[] GetVisiblePositions(HexVector origin, int radius);
}