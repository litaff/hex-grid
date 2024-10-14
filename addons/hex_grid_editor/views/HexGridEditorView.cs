namespace hex_grid.addons.hex_grid_editor.views;

using Godot;

[Tool]
public partial class HexGridEditorView : Control
{
    [Export]
    public TabContainer TabContainer { get; private set; }
    [Export]
    public DebugFovView DebugFov { get; private set; }
    [Export]
    public HexEditorView HexEditor { get; private set; }
    [Export]
    public IndicatorGridView IndicatorGrid { get; private set; }
    [Export]
    public ChunkDisplayView ChunkDisplay { get; private set; }
}