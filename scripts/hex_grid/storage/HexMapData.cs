namespace hex_grid.scripts.hex_grid.storage;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using hex;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class HexMapData : Resource
{
    [Export(PropertyHint.MultilineText)]
    public string SerializedMap { get; private set; }
    
    private Dictionary<int, CubeHex> map = new();

    private JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };
    
    public void Serialize()
    {
        SerializedMap = JsonSerializer.Serialize(map, options);
    }
    
    public Dictionary<int, CubeHex> Deserialize()
    {
        if (string.IsNullOrEmpty(SerializedMap)) return map;
        var deserializedMap = JsonSerializer.Deserialize<Dictionary<int, CubeHex>>(SerializedMap, options);
        map = new Dictionary<int, CubeHex>();
        // Generate new hash codes for this execution of the application.
        foreach (var value in deserializedMap.Values)
        {
            map.Add(value.Position.GetHashCode(), value);
        }
        return map;
    }
    
    public void Clear()
    {
        map.Clear();
        SerializedMap = string.Empty;
    }
}