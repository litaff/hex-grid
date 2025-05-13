namespace HexGrid.Map.Hex;

using System.Text.Json.Serialization;
using Godot;

public struct MeshData
{
    public int MeshIndex { get; private set; }
    public int Rotation { get; private set; }
    
    [JsonIgnore]
    public float Radians => Mathf.DegToRad(Rotation);
    
    [JsonConstructor]
    public MeshData(int meshIndex, int rotation)
    {
        MeshIndex = meshIndex;
        Rotation = rotation;
    }
}