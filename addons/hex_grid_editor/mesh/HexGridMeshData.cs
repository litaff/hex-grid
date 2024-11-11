namespace hex_grid.addons.hex_grid_editor.mesh;

using Godot;

public struct HexGridMeshData(GridMapMeshData gridMapMeshData, Material material)
{
    public GridMapMeshData GridMapMeshData { get; } = gridMapMeshData;
    public Material Material { get; } = material;
}