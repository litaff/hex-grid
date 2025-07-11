namespace addons.hex_grid_map_editor;

using editor_grid_indicator;
using fov_display;
using Godot;
using hex_editor;

[Tool]
public partial class HexGridEditorView : Control
{
    [Export]
    public TabContainer TabContainer { get; private set; } = null!;
    [Export]
    public FovDisplayView FovDisplay { get; private set; } = null!;
    [Export]
    public HexEditorView HexEditor { get; private set; } = null!;
    [Export]
    public EditorGridIndicatorView EditorGridIndicator { get; private set; } = null!;
}