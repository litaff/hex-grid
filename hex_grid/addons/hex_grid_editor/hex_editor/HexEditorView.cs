namespace addons.hex_grid_editor.hex_editor;

using System;
using Godot;
using Godot.Collections;
using HexGridMap;
using DoubleConfirmButton = ui.DoubleConfirmButton;

[Tool]
public partial class HexEditorView : Control
{
    [Export]
    public ItemList MeshList { get; private set; } = null!;
    [Export]
    public DoubleConfirmButton LayerResetButton { get; private set; } = null!;
    [Export]
    public DoubleConfirmButton MapResetButton { get; private set; } = null!;
    [Export]
    public SpinBox CurrentLayer { get; private set; } = null!;
    [Export]
    public HexPropertiesView HexProperties { get; private set; } = null!;
    
    public event Action<int>? OnMeshSelected;
    public event Action<int>? OnLayerChanged;
    
    public void Initialize(int previousLayer)
    {
        MeshList.ItemSelected += OnMeshSelectedHandler;
        CurrentLayer.ValueChanged += OnLayerChangedHandler;
        CurrentLayer.Value = previousLayer;
    }

    public new void Dispose()
    {        
        CurrentLayer.ValueChanged -= OnLayerChangedHandler;
        MeshList.ItemSelected -= OnMeshSelectedHandler;
    }

    public void UpdateList(MeshLibrary? meshLibrary)
    {
        MeshList.Clear();
        
        if (meshLibrary == null) return;
        
        foreach (var itemId in meshLibrary.GetItemList())
        {
            MeshList.AddItem(meshLibrary.GetItemName(itemId), meshLibrary.GetItemPreview(itemId));
        }
    }

    public void SelectMesh(int libraryIndex)
    {
        MeshList.Select(libraryIndex);
        OnMeshSelectedHandler(libraryIndex);
    }

    private void OnLayerChangedHandler(double value)
    {
        OnLayerChanged?.Invoke((int)value);
    }

    private void OnMeshSelectedHandler(long index)
    {
        OnMeshSelected?.Invoke((int)index);
    }
}