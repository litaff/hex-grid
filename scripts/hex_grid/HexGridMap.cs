namespace hex_grid.scripts.hex_grid;

using System;
using System.Collections.Generic;
using Godot;
using hex;
using mesh;
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
    public float CellSize { get; private set; }
    [Export]
    public int ChunkSize { get; private set; }
    [Export]
    private Material defaultMaterial;
    [Export]
    public MeshLibrary MeshLibrary { get; private set; }

    [Export, ExportGroup(EDITOR)]
    public int EditorGridSize { get; private set; } = 5;
    [Export, ExportGroup(EDITOR)]
    public bool EditorGridAlphaFalloff { get; private set; } = true;
    [Export, ExportGroup(EDITOR)]
    public bool DisplayChunks { get; private set; } = true;
    
    private HexMapStorage storage;
    private HexMapChunkStorage chunkStorage;
    private PrimitiveHexGridMesh primitiveHexMesh;

    public float HexWidth => 3f / 2f * CellSize;
    public float HexHeight => Mathf.Sqrt(3) * CellSize;

    public HexMapStorage InitializeStorage()
    {
        storage ??= new HexMapStorage();
        if (chunkStorage != null && chunkStorage.IsUpToDate(ChunkSize)) return storage;
        chunkStorage = new HexMapChunkStorage(ChunkSize, MeshLibrary, GetWorld3D());
        foreach (var hex in storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
        return storage;
    }
    
    public void UpdateDebugMesh()
    {
        primitiveHexMesh ??= new PrimitiveHexGridMesh(GetWorld3D(), CellSize, defaultMaterial);
        primitiveHexMesh.UpdateMesh(storage.GetMap());
    }
    
    public void RemoveHex(CubeHexVector hexPosition)
    {
        storage.Remove(hexPosition);
        chunkStorage.RemoveHex(hexPosition);
        GD.Print(chunkStorage);
    }

    public void AddHex(CubeHexVector hexPosition, int meshIndex)
    {
        storage.Add(hexPosition, CellSize, meshIndex);
        chunkStorage.AssignHex(storage.Get(hexPosition));
        GD.Print(chunkStorage);
    }
}