namespace hex_grid.scripts.hex_grid.chunk;

using System.Collections.Generic;
using System.Linq;
using Godot;
using hex;
using vector;

public class CubeChunk
{
    public CubeHexVector Position { get; private set; }
    public int Size { get; private set; }
    
    private List<MultiMeshInstance> meshInstances = new();
    private List<CubeHex> hexes = new();
    
    public bool IsEmpty => hexes.Count == 0;
    
    public CubeChunk(CubeHexVector position, int size)
    {
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

    public void UpdateMesh(MeshLibrary meshLibrary, World3D scenario)
    {
        if (meshLibrary == null) return;
        
        ClearMeshInstances();
        
        var sortedHexes = SortHexes();
        foreach (var hexGroup in sortedHexes)
        {
            var firstHex = hexGroup.Value.First();
            var mesh = meshLibrary.GetItemMesh(hexGroup.Key);
            var hexPositions = hexGroup.Value.Select(hex => hex.Position).ToList();
            var relativeHexPositions = hexPositions.Select(position => position.RelativeToChunk(Size)).ToList();
            var relativeWorldPositions =
                relativeHexPositions.Select(position => position.ToWorldPosition(firstHex.Size)).ToList();
            var worldPosition = Position.FromChunkPosition(Size).ToWorldPosition(firstHex.Size);
            var meshInstance = new MultiMeshInstance(mesh, worldPosition, relativeWorldPositions, scenario);
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

    private Dictionary<int, List<CubeHex>> SortHexes()
    {
        Dictionary<int, List<CubeHex>> sortedHexes = new();
        
        foreach (var hex in hexes)
        {
            if (sortedHexes.ContainsKey(hex.LibraryIndex))
            {
                sortedHexes[hex.LibraryIndex].Add(hex);
            }
            else
            {
                sortedHexes.Add(hex.LibraryIndex, new List<CubeHex> {hex});
            }
        }

        return sortedHexes;
    }

    public void Dispose()
    {
        ClearMeshInstances();
    }
}