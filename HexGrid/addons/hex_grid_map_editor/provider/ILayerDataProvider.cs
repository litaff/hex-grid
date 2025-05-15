namespace addons.hex_grid_map_editor.provider;

using Godot;

public interface ILayerDataProvider
{
    public Vector3 CurrentLayerOffset { get; }
    public int CurrentLayer { get; }
}