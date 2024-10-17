namespace hex_grid.scripts.hex_grid.layer;

using System.Linq;
using Godot;
using Godot.Collections;
using hex;
using storage;
using vector;

public class LayerStorage
{
    private float cellSize;
    private HexMapData mapData;
    private HexMapStorage storage;

    private int chunkSize;
    private float layerOffset;
    private Dictionary<HexType, MeshLibrary> libraries;
    private World3D scenario;
    private HexMapChunkStorage chunkStorage;

    public void InitializeHexStorage(float cellSize, HexMapData data)
    {
        storage ??= new HexMapStorage(cellSize, data);
        mapData = data;
        this.cellSize = cellSize;
    }
    
    public void InitializeChunkStorage(int chunkSize, float layerOffset, Dictionary<HexType, MeshLibrary> libraries, 
        World3D scenario)
    {
        chunkStorage?.Dispose();
        chunkStorage = new HexMapChunkStorage(chunkSize, layerOffset, libraries, scenario);
        
        this.chunkSize = chunkSize;
        this.layerOffset = layerOffset;
        this.libraries = libraries;
        this.scenario = scenario;
        
        if (storage == null) return;
        
        foreach (var hex in storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
    }
    
    public void UpdateCellSize(float cellSize)
    {
        this.cellSize = cellSize;
        storage.UpdateCellSize(cellSize);
    }

    public CubeHex GetHex(CubeHexVector hexPosition)
    {
        return storage.Get(hexPosition);
    }
    
    public CubeHex AddHex(CubeHexVector hexPosition, HexMeshData meshData, HexType type)
    {
        var hex = storage.Add(hexPosition, meshData, type);
        chunkStorage.AssignHex(storage.Get(hexPosition));
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
        InitializeHexStorage(cellSize, mapData);
        InitializeChunkStorage(chunkSize, layerOffset, libraries, scenario);
    }

    public bool IsEmpty()
    {
        return storage.GetMap().Length <= 0;
    }

    public void Dispose()
    {
        chunkStorage?.Dispose();
    }
}