namespace HexGridMap.Hex;

using System.Text.Json.Serialization;
using Vector;

public struct HexProperties
{
    public float Height { get; private set; }
    public bool IsOccluder { get; set; }

    public HexProperties( float height)
    {
        Height = height;
    }

    [JsonConstructor]
    public HexProperties(float height, bool isOccluder)
    {
        Height = height;
        IsOccluder = isOccluder;
    }
}