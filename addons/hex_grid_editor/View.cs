namespace hex_grid.addons.hex_grid_editor;

using Godot;

[Tool]
public partial class View : Control
{
    [Export]
    public ItemList ItemList { get; private set; }
    
    public void UpdateList(MeshLibrary meshLibrary)
    {
        if (meshLibrary == null) return;
        
        foreach (var itemId in meshLibrary.GetItemList())
        {
            ItemList.AddItem(meshLibrary.GetItemName(itemId), meshLibrary.GetItemPreview(itemId));
        }
    }
}