namespace HexGridMap.Hex;

using System.Text.Json.Serialization;
using Vector;

public class AccessibleHex : CubeHex
{
    [JsonIgnore]
    public override HexType Type => HexType.Accessible;

    public AccessibleHex(int q, int r, HexMeshData meshData) : base(q, r, meshData)
    {
    }

    [JsonConstructor]
    public AccessibleHex(CubeHexVector position, HexMeshData meshData, bool isOccluder) : 
        base(position, meshData, isOccluder)
    {
    }
}