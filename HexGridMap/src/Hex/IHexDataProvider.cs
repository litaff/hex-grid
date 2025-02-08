namespace HexGridMap.Hex;

public interface IHexDataProvider
{
    public IHexData AddData(int index);
    public void RemoveData(int index);
    public IHexData? GetData(int index);
    public Dictionary<int, IHexData> GetData();
    public void ClearData();
}