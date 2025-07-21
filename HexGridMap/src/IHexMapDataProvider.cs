namespace HexGrid.Map;

using Hex;

public interface IHexMapDataProvider
{
    public IHexMapData GetData();
    public void ClearData();
}