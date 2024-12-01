namespace hex_grid_map.storage;

public interface IHexMapDataProvider
{
    public IHexMapData AddMap(int index);
    public void RemoveMap(int index);
    public IHexMapData? GetMap(int index);
    public Dictionary<int, IHexMapData> GetMaps();
    public void ClearMaps();
}