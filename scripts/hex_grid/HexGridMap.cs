namespace hex_grid.scripts.hex_grid;

using System;
using Godot;
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
    public MeshLibrary MeshLibrary
    {
        get => meshLibrary;
        private set
        {
            meshLibrary = value;
            if (value == null)
            {
                GD.PrintErr("MeshLibrary is null, chunks won't be updated!");
            }
            OnPropertyChange?.Invoke();
        } 
    }

    private float cellSize;
    private int chunkSize;
    private MeshLibrary meshLibrary;
    
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
    
    public HexMapStorage Initialize()
    {
        storage ??= new HexMapStorage();
        if (chunkStorage != null && chunkStorage.IsUpToDate(ChunkSize, MeshLibrary, GetWorld3D())) return storage;
        InitializeChunkStorage();
        return storage;
    }

    private void InitializeChunkStorage()
    {
        chunkStorage?.Dispose();
        chunkStorage = new HexMapChunkStorage(ChunkSize, MeshLibrary, GetWorld3D());
        if (storage == null) return;
        
        foreach (var hex in storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
    }

    public void RemoveHex(CubeHexVector hexPosition)
    {
        storage.Remove(hexPosition);
        chunkStorage.RemoveHex(hexPosition);
    }

    public void AddHex(CubeHexVector hexPosition, int meshIndex)
    {
        storage.Add(hexPosition, CellSize, meshIndex);
        chunkStorage.AssignHex(storage.Get(hexPosition));
    }
}