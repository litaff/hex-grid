namespace hex_grid.scripts.hex_grid.fov;

using vector;

public interface IFovProvider
{
    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius, int layerIndex);
}