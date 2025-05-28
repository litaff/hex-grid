namespace HexGrid.Map.Hex;

using System.Collections.Generic;
using System.Linq;
using Vector;

public class HexMap
{
    private readonly IHexMapData data;

    public HexMap(IHexMapData data)
    {
        this.data = data;
        data.Deserialize();
    }

    public void Add(IHexMapData data)
    {
        data.Deserialize();
        foreach (var hex in data.Map.Values)
        {
            this.data.Map.TryAdd(hex.Position.GetHashCode(), hex);
        }
        this.data.Serialize();
    }
    
    public void Add(Hex hex)
    {
        data.Map.TryAdd(hex.Position.GetHashCode(), hex);
        data.Serialize();
    }

    public void Remove(HexVector position)
    {
        data.Map.Remove(position.GetHashCode());
        data.Serialize();
    }

    public Hex? Get(HexVector position)
    {
        return data.Map.GetValueOrDefault(position.GetHashCode());
    }
    
    public Hex[] GetMap()
    {
        return data.Map.Values.ToArray();
    }
}