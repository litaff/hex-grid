namespace hex_grid.scripts.hex_grid.grid_object.providers;

using vector;

public interface IHexStateProvider
{
    public float GetHexHeight(CubeHexVector position);
    public bool Contains<T>(CubeHexVector position);
    public bool HexIs<T>(CubeHexVector position);
}