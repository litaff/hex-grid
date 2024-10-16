namespace hex_grid.scripts;

using System;
using System.Collections.Generic;
using Godot;

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
    public MeshLibrary MeshLibrary { get; private set; }

    [Export, ExportGroup(EDITOR)]
    public int EditorGridSize { get; private set; } = 5;
    [Export, ExportGroup(EDITOR)]
    public bool EditorGridAlphaFalloff { get; private set; } = true;
    
    private List<MeshInstance3D> meshInstances = new();
    private MeshInstance3D currentMesh;
    private HexMapStorage storage = new();
    
    public float HexWidth => 3f / 2f * cellSize;
    public float HexHeight => Mathf.Sqrt(3) * cellSize;
    public Vector2 QBasis => new(3f / 2f, Mathf.Sqrt(3) / 2f);
    public Vector2 RBasis => new(0, Mathf.Sqrt(3));
    
    /// <summary>
    /// Returns a list of hex positions between two hexes.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public CubeHexVector[] GetLine(CubeHex source, CubeHex target)
    {
        var distance = source.Position.Distance(target.Position);
        var lineHexes = new CubeHexVector[distance + 1];
        for (var i = 0; i <= distance; i++)
        {
            var vector = Round(Lerp(source, target, 1f / distance * i));
            lineHexes[i] = vector;
        }
        return lineHexes;
    }

    public CubeHexVector Round(CubeHexFracVector fraction)
    {
        var q = (int)Math.Round(fraction.Q);
        var r = (int)Math.Round(fraction.R);
        var s = (int)Math.Round(fraction.S);
        
        var qDiff = Math.Abs(q - fraction.Q);
        var rDiff = Math.Abs(r - fraction.R);
        var sDiff = Math.Abs(s - fraction.S);
        
        if (qDiff > rDiff && qDiff > sDiff)
        {
            q = -r - s;
        }
        else if (rDiff > sDiff)
        {
            r = -q - s;
        }

        return new CubeHexVector(q, r);
    }

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
        return Round(new CubeHexFracVector(q, r));
    }

    /// <summary>
    /// Returns a list of hex positions in a ring around a center hex.
    /// The ring starts at the north direction and goes clockwise.
    /// </summary>
    /// <param name="center">Center position of the ring.</param>
    /// <param name="radius">Radius of the ring.</param>
    public CubeHexVector[] GetRing(CubeHexVector center, int radius)
    {
        var directions = new[]
        {
            CubeHexVector.SouthEast, CubeHexVector.South, CubeHexVector.SouthWest, 
            CubeHexVector.NorthWest, CubeHexVector.North, CubeHexVector.NorthEast
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
    /// <param name="hex">Reference hex position.</param>
    /// <param name="direction">Hex direction.</param>
    public CubeHexVector GetNeighbor(CubeHexVector hex, CubeHexVector direction)
    {
        return hex + direction;
    }
    
    private CubeHexFracVector Lerp(CubeHex a, CubeHex b, float t)
    {
        return new CubeHexFracVector(a.Position.Q + (b.Position.Q - a.Position.Q) * t, a.Position.R + (b.Position.R - a.Position.R) * t);
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