namespace addons.hex_grid_editor.hex_editor;

using Godot;
using HexGridMap.Hex;

[Tool]
[GlobalClass]
public partial class HexPropertiesView : Control
{
    [Export]
    protected CheckButton IsOccluderButton { get; private set; } = null!;
    [Export]
    public SpinBox HexHeight { get; private set; } = null!;
    
    public HexProperties GetProperties()
    {
        return new HexProperties((float)HexHeight.Value, IsOccluderButton.ButtonPressed);
    }

    public void SetFromProperties(HexProperties properties)
    {
        IsOccluderButton.ButtonPressed = properties.IsOccluder;
        HexHeight.Value = properties.Height;
    }
}