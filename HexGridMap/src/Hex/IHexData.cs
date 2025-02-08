namespace HexGridMap.Hex;

public interface IHexData
{
    public void Serialize();
    public Dictionary<int, CubeHex> Deserialize();
    public void Clear();
}