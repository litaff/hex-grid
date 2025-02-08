namespace hex_grid;

using System;
using System.Linq;
using addons.hex_grid_editor;
using Godot;
using Godot.Collections;
using hex_grid_map;
using HexGridMap;
using HexGridMap.Fov;
using HexGridMap.Hex;
using HexGridMap.Vector;
using HexGridObject;
using HexGridObject.Managers;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class HexGrid : Node3D, IHexDataProvider, IHexGridMapEditionProvider, IFovProvider
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
    public MeshLibrary MeshLibrary { get; private set; } = null!;
    [Export]
    public Dictionary<int, HexData> MapData { get; private set; } = new();
    [Export]
    public Array<Node> Objects { get; private set; } = [];

    private float cellSize;
    private int chunkSize;
    private float layerHeight;
    private HexGridProperties hexGridProperties = null!;

    public HexGridMap? HexGridMap { get; private set; }
    public HexGridObjectManager? GridObjectManager { get; private set; }
    public IFovProvider FovProvider => this;
    public World3D World3D => GetWorld3D();

    public event Action? OnPropertyChange;
    
    public override void _Ready()
    {
        base._Ready();
        Initialize();
        
        if (HexGridMap == null) return;
        
        GridObjectManager = new HexGridObjectManager(HexGridMap.Layers.ToDictionary(pair => pair.Key, pair => pair.Value as IHexProvider));
        
        // Temp
        foreach (var @object in Objects)
        {
            if(@object is not IHexGridObjectHolder gridObjectHolder) continue;
            GridObjectManager.AddGridObject(gridObjectHolder.HexGridObject, 0);
        }
    }

    public void Initialize()
    {
        hexGridProperties = new HexGridProperties(CellSize, ChunkSize, LayerHeight);
        HexGridMap?.Dispose();
        HexGridMap = new HexGridMap(MeshLibrary, this, GetWorld3D());
    }

    public IHexData? GetData(int index)
    {
        return CollectionExtensions.GetValueOrDefault(MapData, index);
    }

    public IHexData AddData(int index)
    {
        var map = new HexData();
        MapData.Add(index, map);
        return map;
    }

    public void RemoveData(int index)
    {
        MapData.Remove(index);
    }

    public System.Collections.Generic.Dictionary<int, IHexData> GetData()
    {
        return MapData.ToDictionary(pair => pair.Key, pair => pair.Value as IHexData);
    }

    public void ClearData()
    {
        MapData.Clear();
        NotifyPropertyListChanged();
        Initialize();
    }

    public void ClearMaps()
    {
        ClearData();
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