namespace HexGridMap.Hex;

using System.Text.Json.Serialization;
using Vector;

[JsonDerivedType(typeof(ElevatedHex), typeDiscriminator: nameof(ElevatedHex))]
[JsonDerivedType(typeof(AccessibleHex), typeDiscriminator: nameof(AccessibleHex))]
public class CubeHex
{
    public CubeHexVector Position { get; private set; }
    public HexMeshData MeshData { get; private set; }

    public bool IsOccluder { get; set; }
    
    [JsonIgnore]
    public virtual HexType Type => HexType.Base;
    
    public CubeHex(int q, int r, HexMeshData meshData)
    {
        Position = new CubeHexVector(q, r);
        MeshData = meshData;
    }
    
    [JsonConstructor]
    public CubeHex(CubeHexVector position, HexMeshData meshData, bool isOccluder)
    {
        Position = position;
        MeshData = meshData;
        IsOccluder = isOccluder;
    }
}