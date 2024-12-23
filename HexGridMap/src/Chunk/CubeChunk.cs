namespace HexGridMap.Chunk;

using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtils.MultiMeshInstance;
using Hex;
using Vector;

public class CubeChunk
{
    private readonly float verticalOffset;
    private readonly List<MultiMeshInstance> meshInstances = [];
    private readonly List<CubeHex> hexes = [];

    public CubeHexVector Position { get; }
    public bool IsEmpty => hexes.Count == 0;
    public IReadOnlyList<CubeHex> AssignedHexes => hexes;
    
    public CubeChunk(CubeHexVector position, float verticalOffset)
    {
        this.verticalOffset = verticalOffset;
        Position = position;
    }
    
    public void Add(CubeHex hex)
    {
        if (hexes.Find(match => match.Position == hex.Position) != null) return;
        hexes.Add(hex);
    }
    
    public void Remove(CubeHexVector position)
    {
        var hex = hexes.Find(match => match.Position == position);
        if (hex == null) return;
        hexes.Remove(hex);
    }

    public void UpdateMesh(MeshLibrary library, World3D scenario)
    {
        ClearMeshInstances();
        
        var sortedHexes = SortHexes();
        foreach (var hexGroup in sortedHexes)
        {
            var mesh = library.GetItemMesh(hexGroup.Key);
            if (mesh == null) continue;
            var hexRotations = hexGroup.Value.Select(hex => hex.MeshData.Radians).ToList();
            var hexPositions = hexGroup.Value.Select(hex => hex.Position).ToList();
            var relativeHexPositions = hexPositions.Select(position => position.RelativeToChunk()).ToList();
            var relativeWorldHexPositions = relativeHexPositions.Select(position => 
                position.ToWorldPosition()).ToList();
            var transforms = hexRotations.Zip(relativeWorldHexPositions, (rotation, position) => 
                new Transform3D(Basis.Identity.Rotated(Vector3.Up, rotation), position)).ToList();
            var chunkWorldPosition = Position.FromChunkPosition().ToWorldPosition();
            var meshInstance = new MultiMeshInstance(mesh, chunkWorldPosition + Vector3.Up * verticalOffset, transforms, scenario);
            meshInstances.Add(meshInstance);
        }
    }

    public void ClearMeshInstances()
    {
        foreach (var meshInstance in meshInstances)
        {
            meshInstance.Dispose();
        }

        meshInstances.Clear();
    }

    public override string ToString()
    {
        return $"Position: {Position}, Hexes: {hexes.Count}, MeshInstances: {meshInstances.Count}";
    }

    public void Display()
    {
        foreach (var meshInstance in meshInstances)
        {
            meshInstance.Display();
        }
    }

    public void Hide()
    {
        foreach (var meshInstance in meshInstances)
        {
            meshInstance.Hide();
        }
    }

    private Dictionary<int, List<CubeHex>> SortHexes()
    {
        Dictionary<int, List<CubeHex>> sortedHexes = new();
        
        foreach (var hex in hexes)
        {
            var key = hex.MeshData.MeshIndex;
            if (sortedHexes.TryGetValue(key, out var value))
            {
                value.Add(hex);
            }
            else
            {
                sortedHexes.Add(key, [hex]);
            }
        }

        return sortedHexes;
    }

    public void Dispose()
    {
        ClearMeshInstances();
    }

    ~CubeChunk()
    {
        Dispose();
    }
}