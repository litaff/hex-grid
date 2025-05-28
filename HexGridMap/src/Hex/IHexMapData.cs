namespace HexGrid.Map.Hex;

public interface IHexMapData
{
    public Dictionary<int, Hex> Map { get; }
    
    public void Serialize();
    public void Deserialize();
    public void Clear();
}