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

    public void Save()
    {
        if (mapData == null)
        {
            GD.PushWarning("Map wasn't saved. Map data is null.");
            return;
        }
        mapData.Serialize();
    }
    
    public void UpdateCellSize(float cellSize)
    {
        foreach (var hex in map.Values)
        {
            hex.SetSize(cellSize);
        }
    }
    
    public CubeHex Add(CubeHexVector position, float size, HexMeshData meshData, HexType type)
    {
        var hex = type switch
        {
            HexType.Accessible => new AccessibleHex(position.Q, position.R, size, meshData),
            HexType.Base => new CubeHex(position.Q, position.R, size, meshData),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        Add(hex);
        return hex;
    }
    
    public void Remove(CubeHexVector position)
    {
        map.Remove(position.GetHashCode());
    }
    
    public void Add(CubeHex value)
    {
        map.TryAdd(value.Position.GetHashCode(), value);
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