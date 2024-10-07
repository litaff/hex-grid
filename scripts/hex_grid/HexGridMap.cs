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
    private float cellSize;
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

    public float HexWidth => 3f / 2f * cellSize;
    public float HexHeight => Mathf.Sqrt(3) * cellSize;
    public Vector2 QBasis => new(3f / 2f, Mathf.Sqrt(3) / 2f);
    public Vector2 RBasis => new(0, Mathf.Sqrt(3));

    public Vector3 GetWorldPosition(CubeHexVector hexPosition)
    {
        var x = cellSize * (QBasis.X * hexPosition.Q + RBasis.X * hexPosition.R);
        var z = cellSize * (QBasis.Y * hexPosition.Q + RBasis.Y * hexPosition.R);
        return new Vector3(x, 0, z);
    }
    
    public Vector3[] GetHexVertices(Vector3 worldPosition)
    {
        var vertices = new Vector3[6];
        for (var i = 0; i < 6; i++)
        {
            var angle = Mathf.DegToRad(60 * i);
            var x = worldPosition.X + cellSize * Mathf.Cos(angle);
            var z = worldPosition.Z + cellSize * Mathf.Sin(angle);
            vertices[i] = new Vector3(x, 0, z);
        }
        return vertices;
    }

    public CubeHexVector GetHexPosition(Vector3 worldPosition)
    {
        var q = 2f / 3f * worldPosition.X / cellSize;
        var r = (-1f / 3f * worldPosition.X + Mathf.Sqrt(3) / 3f * worldPosition.Z) / cellSize;
        return new CubeHexFracVector(q, r).Round();
    }

    /// <summary>
    /// Returns a list of hex positions in a ring around a center hex.
    /// The ring starts at the north direction and goes clockwise.
    /// </summary>
    public CubeHexVector[] GetRing(CubeHexVector center, int radius)
    {
        var directions = new[]
        {
            CubeHexVector.EastSouth, CubeHexVector.South, CubeHexVector.WestSouth, 
            CubeHexVector.WestNorth, CubeHexVector.North, CubeHexVector.EastNorth
        };
        var results = new List<CubeHexVector>();
        var hex = GetNeighbor(center, CubeHexVector.North * radius);
        for (var i = 0; i < 6; i++)
        {
            for (var j = 0; j < radius; j++)
            {
                results.Add(hex);
                hex = GetNeighbor(hex, directions[i]);
            }
        }
        return results.ToArray();
    }
    
    public CubeHexVector[] GetSpiral(CubeHexVector center, int radius)
    {
        var results = new List<CubeHexVector> {center};
        for (var i = 1; i <= radius; i++)
        {
            results.AddRange(GetRing(center, i));
        }
        return results.ToArray();
    }
    
    /// <summary>
    /// Returns a position of a neighbor hex in a given direction.
    /// </summary>
    public CubeHexVector GetNeighbor(CubeHexVector hex, CubeHexVector direction)
    {
        return hex + direction;
    }

    public HexMapStorage InitializeStorage()
    {
        storage ??= new HexMapStorage();
        return storage;
    }
    
    public void UpdateMesh()
    {
        primitiveHexMesh ??= new PrimitiveHexGridMesh(this, defaultMaterial);
        primitiveHexMesh.UpdateMesh(storage.GetMap());
    }

    /// <summary>
    /// Converts a hex position to a chunk position.
    /// https://observablehq.com/@sanderevers/hexagon-tiling-of-an-hexagonal-grid#small_to_big
    /// </summary>
    public CubeHexVector ToChunkCoordinates(CubeHexVector hexPosition)
    {
        var shift = 3 * ChunkSize + 2;
        var area = 3 * ChunkSize * ChunkSize + 3 * ChunkSize + 1;
        var intermediateVector = new Vector3I(
            Mathf.FloorToInt((hexPosition.R + shift * hexPosition.Q) / (float)area),
            Mathf.FloorToInt((hexPosition.S + shift * hexPosition.R) / (float)area),
            Mathf.FloorToInt((hexPosition.Q + shift * hexPosition.S) / (float)area));
        var chunkPos = new CubeHexVector(
            Mathf.FloorToInt((1 + intermediateVector.X - intermediateVector.Y) / 3f),
            Mathf.FloorToInt((1 + intermediateVector.Y - intermediateVector.Z) / 3f));
        return chunkPos;
    }

    /// <summary>
    /// Converts a chunk position to a hex position.
    /// https://observablehq.com/@sanderevers/hexmod-representation#center_of
    /// </summary>
    public CubeHexVector FromChunkCoordinates(CubeHexVector chunkPosition)
    {
        return new CubeHexVector(
            (ChunkSize + 1) * chunkPosition.Q - ChunkSize * chunkPosition.S,
            (ChunkSize + 1) * chunkPosition.R - ChunkSize * chunkPosition.Q);
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