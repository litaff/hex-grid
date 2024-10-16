namespace hex_grid.addons.hex_grid_editor.views;

using Godot;
using hex_grid.scripts.hex_grid.hex;

[Tool]
public partial class BaseHexPropertiesView : Control
{
    [Export]
    protected CheckButton IsOccluderButton { get; private set; }
    
    public virtual void Apply(CubeHex spawnedHex)
    {
        spawnedHex.IsOccluder = IsOccluderButton.ButtonPressed;
    }
    
    public virtual void SetFrom(CubeHex hex)
    {
        IsOccluderButton.ButtonPressed = hex.IsOccluder;
    }
}