namespace HexGrid.Map.Renderer.Chunk;

using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtils.MultiMeshInstance;
using Hex;
using Vector;

public class Chunk : IRenderer
{
    private readonly float verticalOffset;
    private readonly List<MultiMeshInstance> meshInstances = [];
    private readonly List<Hex> hexes = [];
    private readonly MeshLibrary? library;
    private readonly World3D? scenario;

    public HexVector Position { get; }
    public bool IsEmpty => Hexes.Count == 0;
    public IReadOnlyList<Hex> Hexes => hexes;
 
    public Chunk(HexVector position, float verticalOffset, MeshLibrary? library = null, World3D? scenario = null)
    {
        this.verticalOffset = verticalOffset;
        Position = position;
        this.library = library;
        this.scenario = scenario;
    }
    
    public void Add(Hex hex)
    {
        if (hexes.Find(match => match.Position == hex.Position) != null) return;
        hexes.Add(hex);
    }
    
    public void Remove(HexVector position)
    {
        var hex = hexes.Find(match => match.Position == position);
        if (hex == null) return;
        hexes.Remove(hex);
    }

    public void Update()
    {
        ClearMeshInstances();

        if (library is null || scenario is null) return;
        
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

    public bool Overlaps(IRenderer renderer)
    {
        return Position == renderer.Position;
    }

    public void Dispose()
    {
        ClearMeshInstances();
    }

    private Dictionary<int, List<Hex>> SortHexes()
    {
        Dictionary<int, List<Hex>> sortedHexes = new();
        
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

    ~Chunk()
    {
        Dispose();
    }
}