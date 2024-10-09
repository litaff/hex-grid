namespace hex_grid.scripts.hex_grid;

using System;
using Godot;
using Godot.Collections;
using hex;
using vector;

[GlobalClass]
#if TOOLS
[Tool]
#endif
public partial class HexGridMap : Node3D
{
    #region Editor Properties

    private const string EDITOR = "Editor";

    #endregion

    [Export]
    public float CellSize {
        get => cellSize;
        private set
        {
            cellSize = value;
            storage?.UpdateCellSize(value);
            InitializeChunkStorage();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public int ChunkSize {
        get => chunkSize;
        private set
        {
            chunkSize = value;
            Initialize();
            OnPropertyChange?.Invoke();
        } 
    }
    [Export]
    public Dictionary<HexType, MeshLibrary> MeshLibraries
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

    private float cellSize;
    private int chunkSize;
    private Dictionary<HexType, MeshLibrary> meshLibraries;
    
    [Export, ExportGroup(EDITOR)]
    public int EditorGridSize
    {
        get => editorGridSize;
        private set
        {
            editorGridSize = value;
            OnPropertyChange?.Invoke();
        } 
    }
    [Export, ExportGroup(EDITOR)]
    public bool EditorGridAlphaFalloff
    {
        get => editorGridAlphaFalloff;
        private set
        {
            editorGridAlphaFalloff = value;
            OnPropertyChange?.Invoke();
        } 
    }
    [Export, ExportGroup(EDITOR)]
    public bool DisplayChunks
    {
        get => displayChunks;
        private set
        {
            displayChunks = value;
            OnPropertyChange?.Invoke();
        } 
    }
    [Export, ExportGroup(EDITOR)]
    public bool DisplayDebugHexes
    {
        get => displayDebugHexes;
        private set
        {
            displayDebugHexes = value;
            OnPropertyChange?.Invoke();
        } 
    }
    
    private int editorGridSize;
    private bool editorGridAlphaFalloff;
    private bool displayChunks;
    private bool displayDebugHexes;
    
    private HexMapStorage storage;
    private HexMapChunkStorage chunkStorage;

    public float HexWidth => 3f / 2f * CellSize;
    public float HexHeight => Mathf.Sqrt(3) * CellSize;
    public CubeHex[] Map => storage?.GetMap();

    public event Action OnPropertyChange;

    public void Initialize()
    {
        storage ??= new HexMapStorage();
        if (chunkStorage != null && chunkStorage.IsUpToDate(ChunkSize, MeshLibraries, GetWorld3D())) return;
        InitializeChunkStorage();
    }

    public void RemoveHex(CubeHexVector hexPosition)
    {
        storage.Remove(hexPosition);
        chunkStorage.RemoveHex(hexPosition);
    }

    public void AddHex(CubeHexVector hexPosition, int meshIndex, HexType type)
    {
        storage.Add(hexPosition, CellSize, meshIndex, type);
        chunkStorage.AssignHex(storage.Get(hexPosition));
    }

    public void ResetMap()
    {
        storage = null;
        chunkStorage?.Dispose();
        chunkStorage = null;
        Initialize();
    }

    private void InitializeChunkStorage()
    {
        chunkStorage?.Dispose();
        chunkStorage = new HexMapChunkStorage(ChunkSize, MeshLibraries, GetWorld3D());
        if (storage == null) return;
        
        foreach (var hex in storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
    }
}