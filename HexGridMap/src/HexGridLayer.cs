namespace HexGridMap;

using System.Linq;
using Chunk;
using Godot;
using Hex;
using Vector;

public class HexGridLayer : IHexProvider
{
    private readonly HexMap hexMap;
    private readonly ChunkMap chunkMap;

    public HexGridLayer(IHexData hexData, int layerIndex, MeshLibrary library,
        World3D scenario)
    {
        hexMap = new HexMap(hexData);
        
        chunkMap = new ChunkMap(layerIndex * HexGridProperties.LayerHeight, library, scenario);
        foreach (var hex in hexMap.GetMap())
        {
            chunkMap.AssignHex(hex);
        }
    }

    public CubeHex? GetHex(CubeHexVector hexPosition)
    {
        return hexMap.Get(hexPosition);
    }
    
    public void AddHex(CubeHex hex)
    {
        hexMap.Add(hex);
        chunkMap.AssignHex(hex);
    }

    public void RemoveHex(CubeHexVector hexPosition)
    {
        hexMap.Remove(hexPosition);
        chunkMap.RemoveHex(hexPosition);
    }

    // TODO: Why is this here? The storage is not a fov provider...
    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius)
    {
        var visiblePositions = new Dictionary<int, CubeHexVector>();
        var edge = origin.GetRing(radius);
        foreach (var edgePoint in edge)
        {
            var visionLine = origin.LineTo(edgePoint);
            var hexes = visionLine.Select(position => hexMap.Get(position)).Where(hex => hex != null);
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
        return hexMap.GetMap().Length <= 0;
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions)
    {
        chunkMap.HideChunks(chunkPositions);
    }

    public void Display()
    {
        chunkMap.HideChunks([]);
    }
    
    public void Hide()
    {
        chunkMap.Hide();
    }
    
    public void Dispose()
    {
        chunkMap.Dispose();
    }

    ~HexGridLayer()
    {
        chunkMap.Dispose();
    }
}