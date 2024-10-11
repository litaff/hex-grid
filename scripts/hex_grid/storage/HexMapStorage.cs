namespace hex_grid.scripts.hex_grid.storage;

using System;
using System.Collections.Generic;
using System.Linq;
using hex;
using vector;

public class HexMapStorage
{
    private Dictionary<int, CubeHex> map;
    
    public HexMapStorage()
    {
        map = new Dictionary<int, CubeHex>();
    }

    public void UpdateCellSize(float cellSize)
    {
        foreach (var hex in map.Values)
        {
            hex.SetSize(cellSize);
        }
    }
    
    public CubeHex Add(CubeHexVector position, float size, int libraryIndex, HexType type)
    {
        var hex = type switch
        {
            HexType.Accessible => new AccessibleHex(position.Q, position.R, size, libraryIndex),
            HexType.Base => new CubeHex(position.Q, position.R, size, libraryIndex),
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
}