namespace hex_grid.addons.hex_grid_editor;

using System;
using Godot;
using Godot.Collections;
using scripts;
using scripts.hex_grid;

[Tool]
public partial class View : Control
{
    [Export]
    public ItemList MeshList { get; private set; }
    [Export]
    public DoubleConfirmButton MapResetButton { get; private set; }
    [Export]
    public OptionButton HexTypeSelector { get; private set; }
    [Export]
    public Dictionary<HexType, NodePath> HexProperties { get; private set; }

    private System.Collections.Generic.Dictionary<HexType, HexTypePropertiesView> propertiesViews = new();
    
    public event Action<HexType, HexTypePropertiesView> OnHexTypeSelected;
    
    public void Initialize()
    {
        base._EnterTree();
        InitializeHexTypeSelector();
        foreach (var hexProperty in HexProperties)
        {
            var hexTypePropertiesView = GetNode<HexTypePropertiesView>(hexProperty.Value);
            propertiesViews.TryAdd(hexProperty.Key, hexTypePropertiesView);
        }
        OnHexTypeSelectedHandler(HexTypeSelector.Selected);
    }
    
    public new void Dispose()
    {
        HexTypeSelector.ItemSelected -= OnHexTypeSelectedHandler;
        HexTypeSelector.Clear();
        foreach (var views in propertiesViews.Values)
        {
            views.Visible = false;
        }
    }

    public void UpdateList(MeshLibrary meshLibrary)
    {
        MeshList.Clear();
        
        if (meshLibrary == null) return;
        
        foreach (var itemId in meshLibrary.GetItemList())
        {
            MeshList.AddItem(meshLibrary.GetItemName(itemId), meshLibrary.GetItemPreview(itemId));
        }
    }

    private void InitializeHexTypeSelector()
    {
        foreach (HexType hexType in Enum.GetValues(typeof(HexType)))
        {
            HexTypeSelector.AddItem(hexType.ToString(), (int)hexType);
        }
        HexTypeSelector.ItemSelected += OnHexTypeSelectedHandler;
    }

    private void OnHexTypeSelectedHandler(long index)
    {
        var hexType = (HexType)index;
        foreach (var views in propertiesViews.Values)
        {
            views.Visible = false;
        }

        var view = propertiesViews[hexType];
        view.Visible = true;
        OnHexTypeSelected?.Invoke(hexType, view);
    }
}