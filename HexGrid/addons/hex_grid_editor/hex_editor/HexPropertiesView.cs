namespace addons.hex_grid_editor.hex_editor;

using Godot;
using HexGrid.Map.Hex;

[Tool]
[GlobalClass]
public partial class HexPropertiesView : Control
{
    [Export]
    protected CheckButton IsOccluderButton { get; private set; } = null!;
    [Export]
    public SpinBox HexHeight { get; private set; } = null!;
    
    public Properties GetProperties()
    {
        return new Properties((float)HexHeight.Value, IsOccluderButton.ButtonPressed);
    }

    public void SetFromProperties(Properties properties)
    {
        IsOccluderButton.ButtonPressed = properties.IsOccluder;
        HexHeight.Value = properties.Height;
    }
}