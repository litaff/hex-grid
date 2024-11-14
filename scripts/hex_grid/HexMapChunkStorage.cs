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
    private readonly Godot.Collections.Dictionary<HexType, MeshLibrary> libraries;
    private readonly World3D scenario;
    private Dictionary<int, CubeChunk> map;
    private float verticalOffset;
    
    private List<CubeChunk> hiddenChunks = [];

    public HexMapChunkStorage(float verticalOffset, Godot.Collections.Dictionary<HexType, MeshLibrary> libraries, World3D scenario)
    {
        map = new Dictionary<int, CubeChunk>();
        this.libraries = libraries;
        this.scenario = scenario;
        this.verticalOffset = verticalOffset;
    }

    public bool IsUpToDate(Godot.Collections.Dictionary<HexType, MeshLibrary> libraries, World3D scenario)
    {
        return this.libraries == libraries && this.scenario == scenario;
    }
    
    public void AssignHex(CubeHex hex)
    {
        var chunkPosition = hex.Position.ToChunkPosition();
        var chunk = Get(chunkPosition);
        if (chunk == null)
        {
            chunk = new CubeChunk(chunkPosition, verticalOffset);
            AddChunk(chunk);
        }
        chunk.Add(hex);
        
        chunk.UpdateMesh(libraries, scenario);
    }
    
    public void RemoveHex(CubeHexVector position)
    {
        var chunkPosition = position.ToChunkPosition();
        var chunk = Get(chunkPosition);
        if (chunk == null) return;
        chunk.Remove(position);
        chunk.UpdateMesh(libraries, scenario);
        if (!chunk.IsEmpty) return;
        RemoveChunk(chunkPosition);
    }
    
    public CubeChunk[] HideChunks(CubeHexVector[] chunkPositions, out List<CubeChunk> displayedChunks)
    {
        displayedChunks = [];
        var chunksToHide = chunkPositions.Select(Get).Where(chunk => chunk != null).ToList();
        foreach (var hiddenChunk in hiddenChunks.Where(hiddenChunk => !chunksToHide.Contains(hiddenChunk)))
        {
            hiddenChunk.Display();
            displayedChunks.Add(hiddenChunk);
        }
        
        hiddenChunks.Clear();
        
        foreach (var chunkToHide in chunksToHide)
        {
            chunkToHide.Hide();
            hiddenChunks.Add(chunkToHide);
        }
        
        return hiddenChunks.ToArray();
    }

    public CubeChunk[] Hide()
    {
        foreach (var chunk in map.Values.Where(chunk => !hiddenChunks.Contains(chunk)))
        {
            chunk.Hide();
            hiddenChunks.Add(chunk);
        }
        return hiddenChunks.ToArray();
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