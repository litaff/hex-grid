namespace hex_grid.scripts.hex_grid;

using System;
using System.Collections.Generic;
using System.Linq;
using fov;
using Godot;
using hex;
using layer;
using storage;
using vector;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class HexGridMap : Node3D, IFovProvider
{
    [Export]
    public Godot.Collections.Dictionary<int, HexMapData> MapData { get; private set; }
    [Export]
    public float CellSize {
        get => cellSize;
        private set
        {
            cellSize = value;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            UpdateCellSize();
            InitializeChunkStorage();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public int ChunkSize {
        get => chunkSize;
        private set
        {
            chunkSize = value > 0 ? value : 1;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            InitializeChunkStorage();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public float LayerHeight {
        get => layerHeight;
        private set
        {
            layerHeight = value > 0 ? value : 1;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            Initialize();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public Godot.Collections.Dictionary<HexType, MeshLibrary> MeshLibraries { get; private set; }

    private float cellSize;
    private int chunkSize;
    private float layerHeight;

    public Dictionary<int, LayerStorage> Layers { get; private set; } = new();

    public float HexWidth => 3f / 2f * CellSize;
    public float HexHeight => Mathf.Sqrt(3) * CellSize;

    public event Action OnPropertyChange;

    public void Initialize()
    {
        foreach (var layer in Layers.Values)
        {
            layer.Dispose();
        }

        Layers.Clear();
        
        foreach (var data in MapData)
        {
            Layers.Add(data.Key, CreateLayer(data.Key, data.Value));
        }
    }

    public CubeHex AddHex(CubeHexVector hexPosition, HexMeshData meshData, HexType type, int layerIndex)
    {
        if (Layers.TryGetValue(layerIndex, out var layer)) return layer.AddHex(hexPosition, meshData, type);

        var data = new HexMapData();
        MapData.Add(layerIndex, data);
        NotifyPropertyListChanged();
        layer = CreateLayer(layerIndex, data);
        Layers.Add(layerIndex, layer);
        return layer.AddHex(hexPosition, meshData, type);
    }
    
    public void RemoveHex(CubeHexVector hexPosition, int layerIndex)
    {
        if (!Layers.TryGetValue(layerIndex, out var layer)) return;
        layer.RemoveHex(hexPosition);
        CleanUpData(layerIndex, layer);
    }

    public void ClearLayer(int layerIndex)
    {
        if (!Layers.TryGetValue(layerIndex, out var layer)) return;
        layer.Clear();
        CleanUpData(layerIndex, layer);
    }
    
    public void ClearMap()
    {
        foreach (var layer in Layers.Values)
        {
            layer.Clear();
        }
        
        Layers.Clear();
        MapData.Clear();
        NotifyPropertyListChanged();
    }

    public CubeHex GetHex(CubeHexVector hexPosition, int layerIndex)
    {
        return Layers.TryGetValue(layerIndex, out var layer) ? layer.GetHex(hexPosition) : null;
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions, int[] layerIndexes)
    {
        foreach (var layerIndex in layerIndexes)
        {
            if (!Layers.TryGetValue(layerIndex, out var layer)) continue;
            layer.HideChunks(chunkPositions);
        }
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions, Predicate<int> layerPredicate)
    {
        foreach (var layer in Layers.Where(layer => layerPredicate(layer.Key)))
        {
            layer.Value.HideChunks(chunkPositions);
        }
    }
    
    public void HideLayers(int[] layerIndexes)
    {
        foreach (var layerIndex in layerIndexes)
        {
            if (!Layers.TryGetValue(layerIndex, out var layer)) continue;
            layer.Hide();
        }
        
        foreach (var layer in Layers.Where(layer => !layerIndexes.Contains(layer.Key)))
        {
            layer.Value.Display();
        }
    }
    
    public void HideLayers(Predicate<int> predicate)
    {
        foreach (var layer in Layers.Where(layer => predicate(layer.Key)))
        {
            layer.Value.Hide();
        }
        
        foreach (var layer in Layers.Where(layer => !predicate(layer.Key)))
        {
            layer.Value.Display();
        }
    }

    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius, int layerIndex)
    {
        return Layers.TryGetValue(layerIndex, out var layer) ? layer.GetVisiblePositions(origin, radius) : [];
    }

    private void CleanUpData(int layerIndex, LayerStorage layer)
    {
        if (!layer.IsEmpty()) return;
        Layers.Remove(layerIndex);
        MapData.Remove(layerIndex);
        NotifyPropertyListChanged();
    }

    private LayerStorage CreateLayer(int layerIndex, HexMapData data)
    {
        var layerStorage = new LayerStorage();
        layerStorage.InitializeHexStorage(CellSize, data);
        layerStorage.InitializeChunkStorage(chunkSize, layerIndex * layerHeight, MeshLibraries, GetWorld3D());
        return layerStorage;
    }

    private void InitializeChunkStorage()
    {
        foreach (var layer in Layers)
        {
            layer.Value.InitializeChunkStorage(ChunkSize, layer.Key * layerHeight, MeshLibraries, GetWorld3D());
        }
    }

    private void UpdateCellSize()
    {
        foreach (var layer in Layers.Values)
        {
            layer.UpdateCellSize(CellSize);
        }
    }
}