namespace addons.hex_grid_map_editor;

using System;
using System.Linq;
using Godot;
using Godot.Collections;
using hex_grid.hex_grid_map;
using HexGrid.Entity;
using HexGrid.Entity.Managers;
using HexGrid.Map;
using HexGrid.Map.Fov;
using HexGrid.Map.Hex;
using HexGrid.Map.Vector;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;
using GridMap = HexGrid.Map.GridMap;
using Properties = HexGrid.Map.Properties;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class HexGridMapEditor : Node3D, IHexMapDataProvider, IHexGridMapEditionProvider
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
    public Dictionary<int, HexMapData> MapData { get; private set; } = new();
    [Export]
    public Array<Node> Entities { get; private set; } = [];

    private float cellSize;
    private int chunkSize;
    private float layerHeight;
    private Properties properties = null!;

    public GridMap? HexGridMap { get; private set; }
    public EntityManager? EntityManager { get; private set; }
    public World3D World3D => GetWorld3D();
    public System.Collections.Generic.Dictionary<int, IFovProvider> FovProviders { get; private set; }

    public event Action? OnPropertyChange;
    
    public override void _Ready()
    {
        base._Ready();
        Initialize();
        
        if (HexGridMap == null) return;
        
        EntityManager = new EntityManager(HexGridMap.Layers.ToDictionary(pair => pair.Key, pair => pair.Value as IHexProvider));
        
        // Temp
        foreach (var entity in Entities)
        {
            if(entity is not IEntityProvider entityProvider) continue;
            EntityManager.Add(entityProvider.Entity, 0);
        }
    }

    public void Initialize()
    {
        properties = new Properties(CellSize, ChunkSize, LayerHeight);
        HexGridMap?.Dispose();
        HexGridMap = new GridMap(MeshLibrary, this, GetWorld3D());
        InitializeFovProviders();
    }

    public IHexMapData? GetData(int index)
    {
        return CollectionExtensions.GetValueOrDefault(MapData, index);
    }

    public IHexMapData AddData(int index)
    {
        var map = new HexMapData();
        MapData.Add(index, map);
        return map;
    }

    public void RemoveData(int index)
    {
        MapData.Remove(index);
    }

    public System.Collections.Generic.Dictionary<int, IHexMapData> GetData()
    {
        return MapData.ToDictionary(pair => pair.Key, pair => pair.Value as IHexMapData);
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

    public void RemoveHex(HexVector hexPosition, int layerIndex)
    {
        if (HexGridMap == null)
        {
            GD.PushError("[StandaloneHexGrid] Tried to remove hexes from uninitialized hex grid map.");
            return;
        }
        HexGridMap.RemoveHex(hexPosition, layerIndex);
        InitializeFovProviders();
        NotifyPropertyListChanged();
    }

    public void AddHex(Hex hex, int layerIndex)
    {
        if (HexGridMap == null)
        {
            GD.PushError("[StandaloneHexGrid] Tried to add hexes to uninitialized hex grid map.");
            return;
        }
        HexGridMap.AddHex(hex, layerIndex);
        InitializeFovProviders();
        NotifyPropertyListChanged();
    }

    public void HideLayers(int[] layerIndexes)
    {
        if (HexGridMap != null)
        {
            HexGridMap.HideLayers(layerIndexes);
            return;
        }
        GD.PushError("[StandaloneHexGrid] Tried to hide layers of uninitialized hex grid map.");
    }

    public void HideLayers(Predicate<int> predicate)
    {
        if (HexGridMap != null)
        {
            HexGridMap.HideLayers(predicate);
            return;
        }
        GD.PushError("[StandaloneHexGrid] Tried to hide layers of uninitialized hex grid map.");
    }

    public Hex? GetHex(HexVector hexPosition, int layerIndex)
    {
        if (HexGridMap != null) return HexGridMap.GetHex(hexPosition, layerIndex);
        GD.PushError("[StandaloneHexGrid] Tried to get hex from uninitialized hex grid map.");
        return null;
    }

    private void InitializeFovProviders()
    {
        FovProviders = new System.Collections.Generic.Dictionary<int, IFovProvider>();
        if (HexGridMap == null) return;
        foreach (var layer in HexGridMap.Layers)
        {
            FovProviders.Add(layer.Key, new LineFov(layer.Value));
        }
    }
}