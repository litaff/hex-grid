namespace hex_grid_map.storage;

using hex;

public interface IHexMapData
{
    public void Serialize();
    public Dictionary<int, CubeHex> Deserialize();
    public void Clear();
}