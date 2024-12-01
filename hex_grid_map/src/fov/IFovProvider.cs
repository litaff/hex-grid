namespace hex_grid_map.fov;

using vector;

public interface IFovProvider
{
    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius, int layerIndex);
}