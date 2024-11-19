namespace hex_grid.addons.hex_grid_editor.hex_editor;

using Godot;
using scripts.hex_grid.hex;

[Tool]
public partial class ElevatedHexPropertiesView : AccessibleHexPropertiesView
{
    [Export]
    public SpinBox HexHeight { get; private set; }
    
    public override void Apply(CubeHex spawnedHex)
    {
        if (spawnedHex is ElevatedHex elevatedHex)
        {
            elevatedHex.SetHeight((float)HexHeight.Value);
        }
        base.Apply(spawnedHex);
    }
    
    public override void SetFrom(CubeHex hex)
    {
        if (hex is ElevatedHex elevatedHex)
        {
            HexHeight.Value = elevatedHex.Height;
        }
        base.SetFrom(hex);
    }
}