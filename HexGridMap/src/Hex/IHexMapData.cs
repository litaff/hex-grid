namespace HexGrid.Map.Hex;

public interface IHexMapData
{
    public void Serialize();
    public Dictionary<int, Hex> Deserialize();
    public void Clear();
}