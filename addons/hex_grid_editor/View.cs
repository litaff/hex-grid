namespace hex_grid.addons.hex_grid_editor;

using Godot;
using scripts;

[Tool]
public partial class View : Control
{
    [Export]
    public ItemList ItemList { get; private set; }
    [Export]
    public DoubleConfirmButton MapResetButton { get; private set; }
    
    public void UpdateList(MeshLibrary meshLibrary)
    {
        if (meshLibrary == null) return;
        
        foreach (var itemId in meshLibrary.GetItemList())
        {
            ItemList.AddItem(meshLibrary.GetItemName(itemId), meshLibrary.GetItemPreview(itemId));
        }
    }
}