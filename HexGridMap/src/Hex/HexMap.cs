namespace HexGrid.Map.Hex;

using System.Collections.Generic;
using System.Linq;
using Vector;

public class HexMap
{
    private readonly IHexMapData mapData;
    private readonly Dictionary<int, Hex> map;

    public HexMap(IHexMapData mapData)
    {
        this.mapData = mapData;
        map = mapData.Deserialize();
    }

    public void Add(Hex hex)
    {
        map.TryAdd(hex.Position.GetHashCode(), hex);
        mapData.Serialize();
    }

    public void Remove(HexVector position)
    {
        map.Remove(position.GetHashCode());
        mapData.Serialize();
    }

    public Hex? Get(HexVector position)
    {
        return map.GetValueOrDefault(position.GetHashCode());
    }
    
    public Hex[] GetMap()
    {
        return map.Values.ToArray();
    }
}