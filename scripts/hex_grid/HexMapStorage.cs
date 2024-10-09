namespace hex_grid.scripts.hex_grid;

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
        var hash = HashCode.Combine(position.Q, position.R);
        map.Remove(hash);
    }
    
    public void Add(CubeHex value)
    {
        var hash = HashCode.Combine(value.Position.Q, value.Position.R);
        map.TryAdd(hash, value);
    }

    public CubeHex Get(int q, int r)
    {
        var hash = HashCode.Combine(q, r);
        return map.GetValueOrDefault(hash);
    }

    public CubeHex Get(CubeHexVector hexPosition)
    {
        return Get(hexPosition.Q, hexPosition.R);
    }
    
    public CubeHex[] GetMap()
    {
        return map.Values.ToArray();
    }
}