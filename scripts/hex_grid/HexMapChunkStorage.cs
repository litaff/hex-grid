namespace hex_grid.scripts.hex_grid;

using System;
using System.Collections.Generic;
using System.Linq;
using chunk;
using Godot;
using hex;
using vector;

public class HexMapChunkStorage
{
    private readonly MeshLibrary library;
    private readonly World3D scenario;
    private Dictionary<int, CubeChunk> map;
    private int chunkSize;

    public HexMapChunkStorage(int chunkSize, MeshLibrary library, World3D scenario)
    {
        this.chunkSize = chunkSize;
        map = new Dictionary<int, CubeChunk>();
        this.library = library;
        this.scenario = scenario;
    }

    public bool IsUpToDate(int chunkSize, MeshLibrary library, World3D scenario)
    {
        return this.chunkSize == chunkSize && this.library == library && this.scenario == scenario;
    }
    
    public void AssignHex(CubeHex hex)
    {
        var chunkPosition = hex.Position.ToChunkPosition(chunkSize);
        var chunk = Get(chunkPosition);
        if (chunk == null)
        {
            chunk = new CubeChunk(chunkPosition, chunkSize);
            AddChunk(chunk);
        }
        chunk.Add(hex);
        
        chunk.UpdateMesh(library, scenario);
    }
    
    public void RemoveHex(CubeHexVector position)
    {
        var chunkPosition = position.ToChunkPosition(chunkSize);
        var chunk = Get(chunkPosition);
        if (chunk == null) return;
        chunk.Remove(position);
        chunk.UpdateMesh(library, scenario);
        if (!chunk.IsEmpty) return;
        RemoveChunk(chunkPosition);
    }
    
    private void AddChunk(CubeChunk value)
    {
        var hash = HashCode.Combine(value.Position.Q, value.Position.R);
        map.TryAdd(hash, value);
    }
    
    private void RemoveChunk(CubeHexVector position)
    {
        var hash = HashCode.Combine(position.Q, position.R);
        map.Remove(hash);
    }

    private CubeChunk Get(int q, int r)
    {
        var hash = HashCode.Combine(q, r);
        return map.GetValueOrDefault(hash);
    }

    private CubeChunk Get(CubeHexVector hexPosition)
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
}