namespace HexGrid.Map.Hex;

using System.Text.Json.Serialization;

public struct Properties
{
    public float Height { get; private set; }
    public bool IsOccluder { get; set; }

    public Properties( float height)
    {
        Height = height;
    }

    [JsonConstructor]
    public Properties(float height, bool isOccluder)
    {
        Height = height;
        IsOccluder = isOccluder;
    }
}