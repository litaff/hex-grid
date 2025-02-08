namespace HexGridMap.Hex;

using System.Collections.Generic;
using System.Linq;
using Vector;

public class HexMap
{
    private readonly IHexData data;
    private readonly Dictionary<int, CubeHex> map;

    public HexMap(IHexData data)
    {
        this.data = data;
        map = data.Deserialize();
    }

    public void Add(CubeHex value)
    {
        map.TryAdd(value.Position.GetHashCode(), value);
        data.Serialize();
    }

    public void Remove(CubeHexVector position)
    {
        map.Remove(position.GetHashCode());
        data.Serialize();
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