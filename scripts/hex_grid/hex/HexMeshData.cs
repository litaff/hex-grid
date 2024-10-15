namespace hex_grid.scripts.hex_grid.hex;

using System.Text.Json.Serialization;
using Godot;

public struct HexMeshData(int meshIndex, int rotation)
{
    public int MeshIndex { get; private set; } = meshIndex;
    public int Rotation { get; private set; } = rotation;
    
    [JsonIgnore]
    public float Radians => Mathf.DegToRad(Rotation);
}