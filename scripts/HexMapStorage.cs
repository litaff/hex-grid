namespace hex_grid.scripts;

using System;
using System.Collections.Generic;

public class HexMapStorage
{
    private Dictionary<int, CubeHex> map;
    
    public HexMapStorage()
    {
        map = new Dictionary<int, CubeHex>();
    }
    
    public void Add(CubeHex value)
    {
        var hash = HashCode.Combine(value.Position.Q, value.Position.R);
        map.Add(hash, value);
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
}