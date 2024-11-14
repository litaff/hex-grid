namespace hex_grid.addons.hex_grid_editor.mesh;

using Godot;

public struct HexGridMeshData(World3D world, Material material)
{
    public World3D World { get; } = world;
    public Material Material { get; } = material;
}