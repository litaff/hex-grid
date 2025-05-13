namespace HexGrid.Map.Chunk;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hex;
using Vector;

public class ChunkMap
{
    private readonly MeshLibrary library;
    private readonly World3D scenario;
    private readonly Dictionary<int, Chunk> map;
    private readonly float verticalOffset;
    private readonly List<Chunk> hiddenChunks = [];

    public ChunkMap(float verticalOffset, MeshLibrary library, World3D scenario)
    {
        map = new Dictionary<int, Chunk>();
        this.library = library;
        this.scenario = scenario;
        this.verticalOffset = verticalOffset;
    }
    
    public void AssignHex(Hex hex)
    {
        var chunkPosition = hex.Position.ToChunkPosition();
        var chunk = Get(chunkPosition);
        if (chunk == null)
        {
            chunk = new Chunk(chunkPosition, verticalOffset);
            AddChunk(chunk);
        }
        chunk.Add(hex);
        
        chunk.UpdateMesh(library, scenario);
    }
    
    public void RemoveHex(HexVector position)
    {
        var chunkPosition = position.ToChunkPosition();
        var chunk = Get(chunkPosition);
        if (chunk == null) return;
        chunk.Remove(position);
        chunk.UpdateMesh(library, scenario);
        if (!chunk.IsEmpty) return;
        RemoveChunk(chunkPosition);
    }
    
    public void HideChunks(HexVector[] chunkPositions)
    {
        var chunksToHide = chunkPositions.Select(Get).Where(chunk => chunk != null).ToList();
        foreach (var hiddenChunk in hiddenChunks.Where(hiddenChunk => !chunksToHide.Contains(hiddenChunk)))
        {
            hiddenChunk.Display();
        }
        
        hiddenChunks.Clear();
        
        foreach (var chunkToHide in chunksToHide.OfType<Chunk>())
        {
            chunkToHide.Hide();
            hiddenChunks.Add(chunkToHide);
        }
    }

    public void Hide()
    {
        foreach (var chunk in map.Values.Where(chunk => !hiddenChunks.Contains(chunk)))
        {
            chunk.Hide();
            hiddenChunks.Add(chunk);
        }
    }

    private void AddChunk(Chunk value)
    {
        var hash = HashCode.Combine(value.Position.Q, value.Position.R);
        map.TryAdd(hash, value);
    }

    private void RemoveChunk(HexVector position)
    {
        var hash = HashCode.Combine(position.Q, position.R);
        map.Remove(hash);
    }

    private Chunk? Get(int q, int r)
    {
        var hash = HashCode.Combine(q, r);
        return map.GetValueOrDefault(hash);
    }

    private Chunk? Get(HexVector hexPosition)
    {
        return Get(hexPosition.Q, hexPosition.R);
    }

    public override string ToString()
    {
        return map.Values.Aggregate("", (current, chunk) => current + (chunk + "\n"));
    }

    public void Dispose()
    {
        foreach (var chunk in map)
        {
            chunk.Value.Dispose();
        }
    }

    ~ChunkMap()
    {
        Dispose();
    }
}