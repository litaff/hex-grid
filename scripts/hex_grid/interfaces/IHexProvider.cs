namespace hex_grid.scripts.hex_grid.interfaces;

using hex;
using vector;

public interface IHexProvider
{
    public CubeHex GetHex(CubeHexVector hexPosition);
}