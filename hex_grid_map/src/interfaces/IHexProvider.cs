namespace hex_grid_map.interfaces;

using hex;
using vector;

public interface IHexProvider
{
    public CubeHex? GetHex(CubeHexVector hexPosition);
}