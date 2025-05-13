namespace HexGrid.Map;

using Hex;

public interface IHexMapDataProvider
{
    public IHexMapData AddData(int index);
    public void RemoveData(int index);
    public IHexMapData? GetData(int index);
    public Dictionary<int, IHexMapData> GetData();
    public void ClearData();
}