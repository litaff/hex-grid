namespace hex_grid_map.layer;

using System.Linq;
using Godot;
using hex;
using interfaces;
using storage;
using vector;

public class LayerStorage : IHexProvider
{
    private readonly HexMapStorage storage;
    private readonly HexMapChunkStorage chunkStorage;

    public LayerStorage(IHexMapData hexMapData, int layerIndex, Dictionary<HexType, MeshLibrary> libraries,
        World3D scenario)
    {
        storage = new HexMapStorage(hexMapData);
        
        chunkStorage = new HexMapChunkStorage(layerIndex * HexGridData.Instance.LayerHeight, libraries, scenario);
        foreach (var hex in storage.GetMap())
        {
            chunkStorage.AssignHex(hex);
        }
    }

    public CubeHex? GetHex(CubeHexVector hexPosition)
    {
        return storage.Get(hexPosition);
    }
    
    public void AddHex(CubeHex hex)
    {
        storage.Add(hex);
        chunkStorage.AssignHex(hex);
    }

    public void RemoveHex(CubeHexVector hexPosition)
    {
        storage.Remove(hexPosition);
        chunkStorage.RemoveHex(hexPosition);
    }

    // TODO: Why is this here? The storage is not a fov provider...
    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius)
    {
        var visiblePositions = new Dictionary<int, CubeHexVector>();
        var edge = origin.GetRing(radius);
        foreach (var edgePoint in edge)
        {
            var visionLine = origin.LineTo(edgePoint);
            var hexes = visionLine.Select(position => storage.Get(position)).Where(hex => hex != null);
            var positions = hexes.TakeWhile(hex => !hex!.IsOccluder).Select(hex => hex!.Position);
            foreach (var position in positions)
            {
                visiblePositions.TryAdd(position.GetHashCode(), position);
            }
        }
        return visiblePositions.Values.ToArray();
    }

    public bool IsEmpty()
    {
        return storage.GetMap().Length <= 0;
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions)
    {
        chunkStorage.HideChunks(chunkPositions);
    }

    public void Display()
    {
        chunkStorage.HideChunks([]);
    }
    
    public void Hide()
    {
        chunkStorage.Hide();
    }
    
    public void Dispose()
    {
        chunkStorage.Dispose();
    }

    ~LayerStorage()
    {
        chunkStorage.Dispose();
    }
}