namespace HexGridMap.Hex;

using System.Text.Json.Serialization;
using Godot;
using Vector;

public class ElevatedHex : AccessibleHex
{
    public float Height { get; private set; }
    
    [JsonIgnore]
    public override HexType Type => HexType.Elevated;

    public ElevatedHex(int q, int r, HexMeshData meshData) : base(q, r, meshData)
    {
    }

    [JsonConstructor]
    public ElevatedHex(CubeHexVector position, HexMeshData meshData, bool isOccluder, float height) : 
        base(position, meshData, isOccluder)
    {
        Height = height;
    }
    
    public void SetHeight(float height)
    {
        if (height < 0)
        {
            GD.PushError("Height cannot be negative!");
            return;
        }
        Height = height;
    }
}