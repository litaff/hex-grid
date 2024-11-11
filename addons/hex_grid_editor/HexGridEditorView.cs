namespace hex_grid.addons.hex_grid_editor;

using Godot;

[Tool]
public partial class HexGridEditorView : Control
{
    [Export]
    public TabContainer TabContainer { get; private set; }
    [Export]
    public fov_display.FovDisplayView FovDisplay { get; private set; }
    [Export]
    public hex_editor.HexEditorView HexEditor { get; private set; }
    [Export]
    public editor_grid_indicator.EditorGridIndicatorView EditorGridIndicator { get; private set; }
    [Export]
    public chunk_display.ChunkDisplayView ChunkDisplay { get; private set; }
}