namespace HexGrid.Map.Mesh;

using Godot;

public struct MeshData(World3D world, Material material)
{
    public World3D World { get; } = world;
    public Material Material { get; } = material;
}