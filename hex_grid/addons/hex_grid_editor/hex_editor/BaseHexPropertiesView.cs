namespace addons.hex_grid_editor.hex_editor;

using Godot;
using HexGridMap.Hex;

[Tool]
public partial class BaseHexPropertiesView : Control
{
    [Export]
    protected CheckButton IsOccluderButton { get; private set; } = null!;
    
    public virtual void Apply(CubeHex spawnedHex)
    {
        spawnedHex.IsOccluder = IsOccluderButton.ButtonPressed;
    }
    
    public virtual void SetFrom(CubeHex hex)
    {
        IsOccluderButton.ButtonPressed = hex.IsOccluder;
    }
}