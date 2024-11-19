namespace hex_grid.scripts.hex_grid.storage;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using hex;
using vector;

public class HexMapStorage
{
    private readonly HexMapData mapData;
    private Dictionary<int, CubeHex> map;

    public HexMapStorage(HexMapData mapData)
    {
        this.mapData = mapData;
        if (mapData != null)
        {
            
            map = mapData.Deserialize();
            return;
        }
        
        GD.PushWarning("Map data is null. Created map won't be saved.");
        map = new Dictionary<int, CubeHex>();
    }
    
    public void Remove(CubeHexVector position)
    {
        map.Remove(position.GetHashCode());
        mapData?.Serialize();
    }
    
    public void Add(CubeHex value)
    {
        map.TryAdd(value.Position.GetHashCode(), value);
        mapData?.Serialize();
    }

    public CubeHex Get(CubeHexVector hexPosition)
    {
        return map.GetValueOrDefault(hexPosition.GetHashCode());
    }
    
    public CubeHex[] GetMap()
    {
        return map.Values.ToArray();
    }

    public void Clear()
    {
        mapData?.Clear();
    }
}