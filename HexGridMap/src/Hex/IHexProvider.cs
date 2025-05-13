namespace HexGrid.Map.Hex;

using Vector;

public interface IHexProvider
{
    public Hex? GetHex(HexVector position);
}