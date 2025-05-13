namespace HexGrid.Map.Hex;

using System.Text.Json.Serialization;
using Vector;

public class Hex
{
    public HexVector Position { get; private set; }
    public Properties Properties { get; private set; }
    public MeshData MeshData { get; private set; }

    [JsonIgnore]
    public float Height => Properties.Height;
    [JsonIgnore]
    public bool IsOccluder => Properties.IsOccluder;
    
    public Hex(int q, int r, Properties properties, MeshData meshData)
    {
        Position = new HexVector(q, r);
        Properties = properties;
        MeshData = meshData;
    }
    
    [JsonConstructor]
    public Hex(HexVector position, Properties properties, MeshData meshData)
    {
        Position = position;
        Properties = properties;
        MeshData = meshData;
    }
}