namespace hex_grid.scripts.hex_grid.chunk;

using System.Collections.Generic;
using System.Linq;
using Godot;
using hex;
using vector;

public class CubeChunk
{
    private float verticalOffset;
    public CubeHexVector Position { get; private set; }
    public int Size { get; private set; }
    
    private List<MultiMeshInstance> meshInstances = new();
    private List<CubeHex> hexes = new();
    
    public bool IsEmpty => hexes.Count == 0;
    public IReadOnlyList<CubeHex> AssignedHexes => hexes;
    
    public CubeChunk(CubeHexVector position, int size, float verticalOffset)
    {
        this.verticalOffset = verticalOffset;
        Position = position;
        Size = size;
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

    public void UpdateMesh(Godot.Collections.Dictionary<HexType, MeshLibrary> libraries, World3D scenario)
    {
        if (libraries == null) return;
        
        ClearMeshInstances();
        
        var sortedHexes = SortHexes();
        foreach (var hexGroup in sortedHexes)
        {
            var hexSize = hexGroup.Value.First().Size;
            var mesh = libraries[hexGroup.Key.Item1]?.GetItemMesh(hexGroup.Key.Item2);
            if (mesh == null) continue;
            var hexRotations = hexGroup.Value.Select(hex => hex.MeshData.Radians).ToList();
            var hexPositions = hexGroup.Value.Select(hex => hex.Position).ToList();
            var relativeHexPositions = hexPositions.Select(position => position.RelativeToChunk(Size)).ToList();
            var relativeWorldHexPositions = relativeHexPositions.Select(position => 
                position.ToWorldPosition(hexSize)).ToList();
            var transforms = hexRotations.Zip(relativeWorldHexPositions, (rotation, position) => 
                new Transform3D(Basis.Identity.Rotated(Vector3.Up, rotation), position)).ToList();
            var chunkWorldPosition = Position.FromChunkPosition(Size).ToWorldPosition(hexSize);
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
        return $"Position: {Position}, Size: {Size}, Hexes: {hexes.Count}, MeshInstances: {meshInstances.Count}";
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

    private Dictionary<(HexType, int), List<CubeHex>> SortHexes()
    {
        Dictionary<(HexType, int), List<CubeHex>> sortedHexes = new();
        
        foreach (var hex in hexes)
        {
            var key = (hex.Type, hex.MeshData.MeshIndex);
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
}