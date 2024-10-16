namespace hex_grid.addons.hex_grid_editor.views;

using System;
using Godot;
using Godot.Collections;
using scripts.hex_grid;
using scripts.ui;

[Tool]
public partial class HexEditorView : Control
{
    [Export]
    public ItemList MeshList { get; private set; }
    [Export]
    public DoubleConfirmButton MapResetButton { get; private set; }
    [Export]
    public OptionButton HexTypeSelector { get; private set; }
    [Export]
    public Dictionary<HexType, NodePath> HexProperties { get; private set; }
    
    private System.Collections.Generic.Dictionary<HexType, BaseHexPropertiesView> propertiesViews = new();
    
    public event Action<HexType, BaseHexPropertiesView> OnHexTypeSelected;
    public event Action<int> OnMeshSelected;
    
    public void Initialize()
    {
        InitializeHexTypeSelector();
        foreach (var hexProperty in HexProperties)
        {
            var hexTypePropertiesView = GetNode<BaseHexPropertiesView>(hexProperty.Value);
            propertiesViews.TryAdd(hexProperty.Key, hexTypePropertiesView);
        }
        OnHexTypeSelectedHandler(HexTypeSelector.Selected);
        MeshList.ItemSelected += OnMeshSelectedHandler;
    }
    
    public new void Dispose()
    {
        HexTypeSelector.ItemSelected -= OnHexTypeSelectedHandler;
        MeshList.ItemSelected -= OnMeshSelectedHandler;
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

    public void SetCurrentHexType(HexType hexType)
    {
        HexTypeSelector.Selected = (int)hexType;
        OnHexTypeSelectedHandler(HexTypeSelector.Selected);
    }

    public void SelectMesh(int libraryIndex)
    {
        MeshList.Select(libraryIndex);
        OnMeshSelectedHandler(libraryIndex);
    }

    private void OnMeshSelectedHandler(long index)
    {
        OnMeshSelected?.Invoke((int)index);
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