namespace hex_grid.scripts.hex_grid.layer;

using System.Linq;
using Godot;
using Godot.Collections;
using hex;
using storage;
using vector;

public class LayerStorage
{
    private HexMapData mapData;
    private HexMapStorage storage;

    private float layerOffset;
    private Dictionary<HexType, MeshLibrary> libraries;
    private World3D scenario;
    private HexMapChunkStorage chunkStorage;

    public void InitializeHexStorage( HexMapData data)
    {
        storage ??= new HexMapStorage(data);
        mapData = data;
    }
    
    public void InitializeChunkStorage(float layerOffset, Dictionary<HexType, MeshLibrary> libraries, 
        World3D scenario)
    {
        chunkStorage?.Dispose();
        chunkStorage = new HexMapChunkStorage(layerOffset, libraries, scenario);
        
        this.layerOffset = layerOffset;
        this.libraries = libraries;
        this.scenario = scenario;
        
        if (storage == null) return;
        
        foreach (var hex in storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
    }

    public CubeHex GetHex(CubeHexVector hexPosition)
    {
        return storage.Get(hexPosition);
    }
    
    public CubeHex AddHex(CubeHex hex)
    {
        storage.Add(hex);
        chunkStorage.AssignHex(hex);
        return hex;
    }

    public void RemoveHex(CubeHexVector hexPosition)
    {
        storage.Remove(hexPosition);
        chunkStorage.RemoveHex(hexPosition);
    }

    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius)
    {
        var visiblePositions = new System.Collections.Generic.Dictionary<int, CubeHexVector>();
        var edge = origin.GetRing(radius);
        foreach (var edgePoint in edge)
        {
            var visionLine = origin.LineTo(edgePoint);
            var hexes = visionLine.Select(position => storage.Get(position)).Where(hex => hex != null);
            var positions = hexes.TakeWhile(hex => !hex.IsOccluder).Select(hex => hex.Position);
            foreach (var position in positions)
            {
                visiblePositions.TryAdd(position.GetHashCode(), position);
            }
        }
        return visiblePositions.Values.ToArray();
    }

    public void Clear()
    {
        storage.Clear();
        storage = null;
        InitializeHexStorage(mapData);
        InitializeChunkStorage(layerOffset, libraries, scenario);
    }

    public bool IsEmpty()
    {
        return storage.GetMap().Length <= 0;
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions)
    {
        // TODO: Hide all entities in the chunks
        var hiddenChunks = chunkStorage.HideChunks(chunkPositions, out var displayedChunks);
    }

    public void Display()
    {
        // TODO: Display all entities in the chunks
        chunkStorage.HideChunks([], out var displayedChunks);
    }
    
    public void Hide()
    {
        // TODO: Hide all entities in the chunks
        var hiddenChunks = chunkStorage.Hide();
    }
    
    public void Dispose()
    {
        chunkStorage?.Dispose();
    }
}