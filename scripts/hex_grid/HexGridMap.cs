namespace hex_grid.scripts.hex_grid;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using hex;
using storage;
using vector;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class HexGridMap : Node3D
{
    [Export]
    public HexMapData MapData
    {
        get => mapData;
        private set
        {
            mapData = value;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            Storage = null;
            Initialize();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public float CellSize {
        get => cellSize;
        private set
        {
            cellSize = value;
            if (!IsInsideTree()) return; // Don't initialize if the node is not in the tree
            Storage?.UpdateCellSize(value);
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
            Initialize();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public Godot.Collections.Dictionary<HexType, MeshLibrary> MeshLibraries
    {
        get => meshLibraries;
        private set
        {
            meshLibraries = value;
            if (value == null)
            {
                GD.PrintErr("MeshLibrary is null, chunks won't be updated!");
            }
            OnPropertyChange?.Invoke();
        } 
    }

    private HexMapData mapData;
    private float cellSize;
    private int chunkSize;
    private Godot.Collections.Dictionary<HexType, MeshLibrary> meshLibraries;
    
    public HexMapStorage Storage { get; private set; }
    private HexMapChunkStorage chunkStorage;

    public float HexWidth => 3f / 2f * CellSize;
    public float HexHeight => Mathf.Sqrt(3) * CellSize;
    public CubeHex[] Map => Storage?.GetMap();

    public event Action OnPropertyChange;
    
    public void Initialize()
    {
        Storage ??= new HexMapStorage(MapData);
        InitializeChunkStorage();
    }

    public void RemoveHex(CubeHexVector hexPosition)
    {
        Storage.Remove(hexPosition);
        chunkStorage.RemoveHex(hexPosition);
    }

    public CubeHex AddHex(CubeHexVector hexPosition, HexMeshData meshData, HexType type)
    {
        var hex = Storage.Add(hexPosition, CellSize, meshData, type);
        chunkStorage.AssignHex(Storage.Get(hexPosition));
        return hex;
    }

    public void ResetMap()
    {
        Storage.Clear();
        Storage = null;
        chunkStorage?.Dispose();
        chunkStorage = null;
        Initialize();
    }

    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius)
    {
        var visiblePositions = new Dictionary<int, CubeHexVector>();
        var edge = origin.GetRing(radius);
        foreach (var edgePoint in edge)
        {
            var visionLine = origin.LineTo(edgePoint);
            var hexes = visionLine.Select(position => Storage.Get(position)).Where(hex => hex != null);
            var positions = hexes.TakeWhile(hex => !hex.IsOccluder).Select(hex => hex.Position);
            foreach (var position in positions)
            {
                visiblePositions.TryAdd(position.GetHashCode(), position);
            }
        }
        return visiblePositions.Values.ToArray();
    }

    private void InitializeChunkStorage()
    {
        chunkStorage?.Dispose();
        chunkStorage = new HexMapChunkStorage(ChunkSize, MeshLibraries, GetWorld3D());
        if (Storage == null) return;
        
        foreach (var hex in Storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
    }
}