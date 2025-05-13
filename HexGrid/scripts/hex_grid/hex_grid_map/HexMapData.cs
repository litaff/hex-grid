namespace hex_grid.hex_grid_map;

using System.Collections.Generic;
using System.Text.Json;
using HexGrid.Map.Hex;
using Godot;

[GlobalClass]
public partial class HexMapData : Resource, IHexMapData
{
    [Export(PropertyHint.MultilineText)]
    public string SerializedMap { get; private set; } = "";
    
    private Dictionary<int, Hex> map = new();

    private JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };
    
    public void Serialize()
    {
        SerializedMap = JsonSerializer.Serialize(map, options);
    }
    
    public Dictionary<int, Hex> Deserialize()
    {
        if (string.IsNullOrEmpty(SerializedMap)) return map;
        var deserializedMap = JsonSerializer.Deserialize<Dictionary<int, Hex>>(SerializedMap, options);
        map = new Dictionary<int, Hex>();

        if (deserializedMap == null)
        {
            GD.PushError($"[HexMapData] Deserialization failed for: {SerializedMap}");
            return map;
        }
        
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