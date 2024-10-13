namespace hex_grid.scripts.hex_grid.hex;

using System.Text.Json.Serialization;
using vector;

public class AccessibleHex : CubeHex
{
    [JsonIgnore]
    public override HexType Type => HexType.Accessible;

    public AccessibleHex(int q, int r, float size, int libraryIndex) : base(q, r, size, libraryIndex)
    {
    }

    [JsonConstructor]
    public AccessibleHex(CubeHexVector position, float size, int libraryIndex, bool isOccluder) : 
        base(position, size, libraryIndex, isOccluder)
    {
    }
}