namespace HexGrid.Map.Renderer.Chunk;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Hex;
using Vector;

public class ChunkMap : IRendererMap
{
    private readonly MeshLibrary? library;
    private readonly World3D? scenario;
    private readonly Dictionary<int, IRenderer> renderers;
    private readonly float verticalOffset;
    private readonly List<IRenderer> hiddenChunks = [];

    public IReadOnlyDictionary<int, IRenderer> Renderers => renderers;
    
    public ChunkMap(float verticalOffset, MeshLibrary? library = null, World3D? scenario = null)
    {
        renderers = new Dictionary<int, IRenderer>();
        this.library = library;
        this.scenario = scenario;
        this.verticalOffset = verticalOffset;
    }

    public void AddTo(Hex[] hexes, HexVector position)
    {
        foreach (var hex in hexes)
        {
            AddHex(hex);
        }
    }

    public void AddHex(Hex hex)
    {
        var chunk = FromHexPosition(hex.Position);
        if (chunk == null)
        {
            chunk = new Chunk(hex.Position.ToChunkPosition(), verticalOffset, library, scenario);
            renderers.TryAdd(chunk.Position.GetHashCode(), chunk);
        }
        chunk.Add(hex);
        
        chunk.Update();
    }
    
    public void RemoveHex(HexVector position)
    {
        var chunk = FromHexPosition(position);
        if (chunk == null) return;
        chunk.Remove(position);
        chunk.Update();
        if (chunk.Hexes.Count != 0) return;
        renderers.Remove(chunk.Position.GetHashCode());
    }

    public void AddRenderer(IRenderer renderer)
    {
        renderers.TryAdd(renderer.Position.GetHashCode(), renderer);
    }

    public void RemoveRenderer(HexVector position)
    {
        renderers.Remove(position.GetHashCode());
    }

    public void Show(HexVector[] rendererPositions)
    {
        foreach (var renderer in renderers.Values)
        {
            if (rendererPositions.Contains(renderer.Position))
            {
                renderer.Display();
                continue;
            }
            renderer.Hide();
        }
    }

    public void Show()
    {
        foreach (var renderer in renderers.Values)
        {
            renderer.Display();
        }
    }

    public IRenderer? FromHexPosition(HexVector position)
    {
        var chunkPosition = position.ToChunkPosition();
        return renderers.GetValueOrDefault(chunkPosition.GetHashCode());
    }

    public override string ToString()
    {
        return renderers.Values.Aggregate("", (current, chunk) => current + (chunk + "\n"));
    }

    public void Dispose()
    {
        foreach (var chunk in renderers)
        {
            chunk.Value.Dispose();
        }
    }

    ~ChunkMap()
    {
        Dispose();
    }
}