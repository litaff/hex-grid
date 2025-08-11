namespace addons.hex_grid_map_editor;

using System;
using System.Linq;
using Godot;
using Godot.Collections;
using hex_grid.grid_objects;
using hex_grid.hex_grid_map;
using HexGrid.Entity;
using HexGrid.Entity.Components;
using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Managers;
using HexGrid.Map;
using HexGrid.Map.Fov;
using HexGrid.Map.Hex;
using HexGrid.Map.Renderer;
using HexGrid.Map.Renderer.Room;
using HexGrid.Map.Vector;
using Blocker = HexGrid.Entity.Components.Blocker.Blocker;
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
    public float HeightStep
    {
        get => heightStep;
        private set
        {
            heightStep = value > 0 ? value : 1;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            Initialize();
            OnPropertyChange?.Invoke();
        }
    }
    [Export]
    public MeshLibrary MeshLibrary { get; private set; } = null!;
    [Export]
    public HexMapData MapData { get; private set; } = new();
    [Export]
    public Array<Node> Entities { get; private set; } = [];

    private float cellSize;
    private float heightStep;
    private Properties properties = null!;

    public GridMap? HexGridMap { get; private set; }
    public EntityContext? EntityContext { get; private set; }
    public World3D World3D => GetWorld3D();
    public IFovProvider FovProvider { get; private set; }

    public event Action? OnPropertyChange;
    
    public override void _Ready()
    {
        base._Ready();
        Initialize();
        
        if (HexGridMap == null) return;

        var entityMap = new EntityManager();
        var hexManager = new HexManager(HexGridMap.HexMap);
        
        var managerRegistry = new ManagerRegistry();
        var heightManager = new HeightManager(entityMap);
        managerRegistry.Add(heightManager);
        var blockerManager = new BlockerManager(entityMap);
        managerRegistry.Add(blockerManager);
        
        EntityContext = new EntityContext(hexManager, entityMap, managerRegistry);

        System.Collections.Generic.Dictionary<IFactoryKey, IEntityFactory> factories = [];
        factories.Add(new BlockerFactoryKey(), new BlockerFactory(EntityContext));
        factories.Add(new PlayerFactoryKey(), new PlayerFactory(EntityContext));
        
        // Temp
        foreach (var node in Entities)
        {
            if(node is not IEntityHolder holder) continue;
            var blueprint = holder.Get();
            var entity = factories[holder.FactoryKey].New(blueprint);
            holder.Set(entity);
            entityMap.Add(entity);
        }
    }

    public void Initialize()
    {
        properties = new Properties(CellSize, HeightStep);
        HexGridMap?.Dispose();
        HexGridMap = new GridMap(this, new RoomMap(MeshLibrary, this));
        InitializeFovProviders();
    }

    public IHexMapData GetData()
    {
        return MapData;
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

    public void ClearMap()
    {
        ClearData();
    }

    public void RemoveHex(HexVector hexPosition)
    {
        if (HexGridMap == null)
        {
            GD.PushError("[HexGridMapEditor] Tried to remove hexes from uninitialized hex grid map.");
            return;
        }
        HexGridMap.RemoveHex(hexPosition);
        InitializeFovProviders();
        NotifyPropertyListChanged();
    }

    public void AddHex(Hex hex)
    {
        if (HexGridMap == null)
        {
            GD.PushError("[HexGridMapEditor] Tried to add hexes to uninitialized hex grid map.");
            return;
        }
        HexGridMap.AddHex(hex);
        InitializeFovProviders();
        NotifyPropertyListChanged();
    }

    public void Occlude(Predicate<HexVector>? predicate)
    {
        if (HexGridMap != null)
        {
            HexGridMap.Occlude(predicate);
            return;
        }
        GD.PushError("[HexGridMapEditor] Tried to hide layers of uninitialized hex grid map.");
    }

    public Hex? GetHex(HexVector hexPosition)
    {
        if (HexGridMap != null) return HexGridMap.GetHex(hexPosition);
        GD.PushError("[HexGridMapEditor] Tried to get hex from uninitialized hex grid map.");
        return null;
    }

    private void InitializeFovProviders()
    {
        if (HexGridMap is null) return;
        FovProvider = new LineFov(HexGridMap.HexMap);
    }
}