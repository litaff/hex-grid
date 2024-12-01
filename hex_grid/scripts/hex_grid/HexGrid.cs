namespace hex_grid;

using System;
using System.Linq;
using addons.hex_grid_editor;
using global::hex_grid_map;
using global::hex_grid_map.fov;
using global::hex_grid_map.hex;
using global::hex_grid_map.interfaces;
using global::hex_grid_map.storage;
using global::hex_grid_map.vector;
using Godot;
using Godot.Collections;
using grid_object;
using grid_object.managers;
using hex_grid_map;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

[Tool]
[GlobalClass]
public partial class HexGrid : Node3D, IHexMapDataProvider, IHexGridMapEditionProvider, IFovProvider
{
    [Export]
    public float CellSize
    {
        get => cellSize;
        private set
        {
            cellSize = value;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            Initialize();
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
            Initialize();
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
    public Dictionary<HexType, MeshLibrary> MeshLibraries { get; private set; } = new();
    [Export]
    public Dictionary<int, HexMapData> MapData { get; private set; } = new();
    [Export]
    public Array<Node> Objects { get; private set; } = [];

    private float cellSize;
    private int chunkSize;
    private float layerHeight;
    private HexGridData hexGridData = null!;

    public HexGridMap? HexGridMap { get; private set; }
    public GridObjectManager? GridObjectManager { get; private set; }
    public IFovProvider FovProvider => this;
    public World3D World3D => GetWorld3D();

    public event Action? OnPropertyChange;
    
    public override void _Ready()
    {
        base._Ready();
        Initialize();
        
        if (HexGridMap == null) return;
        
        GridObjectManager = new GridObjectManager(HexGridMap.Layers.ToDictionary(pair => pair.Key, pair => pair.Value as IHexProvider));
        
        // Temp
        foreach (var @object in Objects)
        {
            if(@object is not IGridObjectHolder gridObjectHolder) continue;
            GridObjectManager.AddGridObject(gridObjectHolder.GridObject, 0);
        }
    }

    public void Initialize()
    {
        hexGridData = new HexGridData(CellSize, ChunkSize, LayerHeight);
        HexGridMap?.Dispose();
        HexGridMap = new HexGridMap(MeshLibraries.ToDictionary(), this, GetWorld3D());
    }

    public IHexMapData? GetMap(int index)
    {
        return CollectionExtensions.GetValueOrDefault(MapData, index);
    }

    public IHexMapData AddMap(int index)
    {
        var map = new HexMapData();
        MapData.Add(index, map);
        return map;
    }

    public void RemoveMap(int index)
    {
        MapData.Remove(index);
    }

    public System.Collections.Generic.Dictionary<int, IHexMapData> GetMaps()
    {
        return MapData.ToDictionary(pair => pair.Key, pair => pair.Value as IHexMapData);
    }

    public void ClearMaps()
    {
        MapData.Clear();
        NotifyPropertyListChanged();
        Initialize();
    }

    public void ClearMap(int index)
    {
        if (!MapData.Remove(index)) return;
        NotifyPropertyListChanged();
        Initialize();
    }

    public void RemoveHex(CubeHexVector hexPosition, int layerIndex)
    {
        if (HexGridMap == null)
        {
            GD.PushError("[HexGrid] Tried to remove hexes from uninitialized hex grid map.");
            return;
        }
        HexGridMap.RemoveHex(hexPosition, layerIndex);
        NotifyPropertyListChanged();
    }

    public void AddHex(CubeHex hex, int layerIndex)
    {
        if (HexGridMap == null)
        {
            GD.PushError("[HexGrid] Tried to add hexes to uninitialized hex grid map.");
            return;
        }
        HexGridMap.AddHex(hex, layerIndex);
        NotifyPropertyListChanged();
    }

    public void HideLayers(int[] layerIndexes)
    {
        if (HexGridMap != null)
        {
            HexGridMap.HideLayers(layerIndexes);
            return;
        }
        GD.PushError("[HexGrid] Tried to hide layers of uninitialized hex grid map.");
    }

    public void HideLayers(Predicate<int> predicate)
    {
        if (HexGridMap != null)
        {
            HexGridMap.HideLayers(predicate);
            return;
        }
        GD.PushError("[HexGrid] Tried to hide layers of uninitialized hex grid map.");
    }

    public CubeHex? GetHex(CubeHexVector hexPosition, int layerIndex)
    {
        if (HexGridMap != null) return HexGridMap.GetHex(hexPosition, layerIndex);
        GD.PushError("[HexGrid] Tried to get hex from uninitialized hex grid map.");
        return null;
    }

    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius, int layerIndex)
    {
        if (HexGridMap != null) return HexGridMap.GetVisiblePositions(origin, radius, layerIndex);
        GD.PushError("[HexGrid] Tried to get fov from uninitialized hex grid map.");
        return [];

    }
}