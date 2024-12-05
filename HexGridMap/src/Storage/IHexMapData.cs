namespace HexGridMap.Storage;

using Hex;

public interface IHexMapData
{
    public void Serialize();
    public Dictionary<int, CubeHex> Deserialize();
    public void Clear();
}