namespace HexGridMap.Mesh;

using Godot;

public struct HexGridMeshData(World3D world, Material material)
{
    public World3D World { get; } = world;
    public Material Material { get; } = material;
}