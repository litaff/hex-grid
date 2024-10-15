namespace hex_grid.scripts.hex_grid.hex;

using System.Text.Json.Serialization;
using vector;

public class AccessibleHex : CubeHex
{
    [JsonIgnore]
    public override HexType Type => HexType.Accessible;

    public AccessibleHex(int q, int r, float size, HexMeshData meshData) : base(q, r, size, meshData)
    {
    }

    [JsonConstructor]
    public AccessibleHex(CubeHexVector position, float size, HexMeshData meshData, bool isOccluder) : 
        base(position, size, meshData, isOccluder)
    {
    }
}