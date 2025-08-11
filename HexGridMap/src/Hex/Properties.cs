namespace HexGrid.Map.Hex;

using System.Text.Json.Serialization;

public struct Properties
{
    public int Height { get; private set; }
    public bool IsOccluder { get; set; }

    public Properties(int height)
    {
        Height = height;
    }

    [JsonConstructor]
    public Properties(int height, bool isOccluder)
    {
        Height = height;
        IsOccluder = isOccluder;
    }
}