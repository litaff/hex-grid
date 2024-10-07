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
    
    private List<MeshInstance3D> meshInstances = new();
    private MeshInstance3D currentMesh;
    private HexMapStorage storage;
    private PrimitiveHexGridMesh primitiveHexMesh;

    public float HexWidth => 3f / 2f * CellSize;
    public float HexHeight => Mathf.Sqrt(3) * CellSize;

    public HexMapStorage InitializeStorage()
    {
        storage ??= new HexMapStorage();
        return storage;
    }
    
    public void UpdateMesh()
    {
        primitiveHexMesh ??= new PrimitiveHexGridMesh(GetWorld3D(), CellSize, defaultMaterial);
        primitiveHexMesh.UpdateMesh(storage.GetMap());
    }

    private void OnMeshSelectedHandler(Mesh mesh)
    {
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = mesh;
        AddChild(meshInstance);
        meshInstances.Add(meshInstance);
        currentMesh = meshInstance;
    }
}