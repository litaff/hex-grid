namespace hex_grid.addons.hex_grid_editor.provider;

using Godot;

public interface ILayerDataProvider
{
    public Vector3 CurrentLayerOffset { get; }
    public int CurrentLayer { get; }
}