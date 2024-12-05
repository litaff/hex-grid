namespace HexGridMap.Storage;

using System.Collections.Generic;
using System.Linq;
using Hex;
using Vector;

public class HexMapStorage
{
    private readonly IHexMapData mapData;
    private readonly Dictionary<int, CubeHex> map;

    public HexMapStorage(IHexMapData mapData)
    {
        this.mapData = mapData;
        map = mapData.Deserialize();
    }

    public void Add(CubeHex value)
    {
        map.TryAdd(value.Position.GetHashCode(), value);
        mapData.Serialize();
    }

    public void Remove(CubeHexVector position)
    {
        map.Remove(position.GetHashCode());
        mapData.Serialize();
    }

    public CubeHex? Get(CubeHexVector hexPosition)
    {
        return map.GetValueOrDefault(hexPosition.GetHashCode());
    }
    
    public CubeHex[] GetMap()
    {
        return map.Values.ToArray();
    }
}