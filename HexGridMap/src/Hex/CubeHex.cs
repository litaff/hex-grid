namespace HexGridMap.Hex;

using System.Text.Json.Serialization;
using Vector;

public class CubeHex
{
    public CubeHexVector Position { get; private set; }
    public HexProperties Properties { get; private set; }
    public HexMeshData MeshData { get; private set; }

    [JsonIgnore]
    public float Height => Properties.Height;
    [JsonIgnore]
    public bool IsOccluder => Properties.IsOccluder;
    
    public CubeHex(int q, int r, HexProperties properties, HexMeshData meshData)
    {
        Position = new CubeHexVector(q, r);
        Properties = properties;
        MeshData = meshData;
    }
    
    [JsonConstructor]
    public CubeHex(CubeHexVector position, HexProperties properties, HexMeshData meshData)
    {
        Position = position;
        Properties = properties;
        MeshData = meshData;
    }
}