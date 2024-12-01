namespace hex_grid_object.providers;

using hex_grid_map.vector;

public interface IHexStateProvider
{
    public float GetHexHeight(CubeHexVector position);
    public bool Contains<T>(CubeHexVector position);
    public bool HexIs<T>(CubeHexVector position);
}