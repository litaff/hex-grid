namespace HexGridMap;

using Hex;
using Vector;

public interface IHexProvider
{
    public CubeHex? GetHex(CubeHexVector hexPosition);
}