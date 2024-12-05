namespace HexGridMap.Interfaces;

using Hex;
using Vector;

public interface IHexProvider
{
    public CubeHex? GetHex(CubeHexVector hexPosition);
}