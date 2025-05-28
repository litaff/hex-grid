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
    
    public Dictionary<int, Hex> Map { get; private set; }

    private JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public HexMapData()
    {
        Map = new Dictionary<int, Hex>();
    }
    
    public void Serialize()
    {
        SerializedMap = JsonSerializer.Serialize(Map, options);
    }
    
    public void Deserialize()
    {
        if (string.IsNullOrEmpty(SerializedMap))
        {
            Map = new Dictionary<int, Hex>();
            return;
        }
        var deserializedMap = JsonSerializer.Deserialize<Dictionary<int, Hex>>(SerializedMap, options);
        Map = new Dictionary<int, Hex>();

        if (deserializedMap == null)
        {
            GD.PushError($"[HexMapData] Deserialization failed for: {SerializedMap}");
            return;
        }
        
        // Generate new hash codes for this execution of the application.
        foreach (var value in deserializedMap.Values)
        {
            Map.Add(value.Position.GetHashCode(), value);
        }
    }
    
    public void Clear()
    {
        Map.Clear();
        SerializedMap = string.Empty;
    }
}