namespace HexGrid.Map;

using Chunk;
using Godot;
using Hex;
using Vector;

public class Layer : IHexProvider
{
    private readonly HexMap hexMap;
    private readonly ChunkMap chunkMap;

    public Layer(IHexMapData hexMapData, int layerIndex, MeshLibrary library,
        World3D scenario)
    {
        hexMap = new HexMap(hexMapData);
        
        chunkMap = new ChunkMap(layerIndex * Properties.LayerHeight, library, scenario);
        foreach (var hex in hexMap.GetMap())
        {
            chunkMap.AssignHex(hex);
        }
    }

    public Hex.Hex? GetHex(HexVector position)
    {
        return hexMap.Get(position);
    }
    
    public void AddHex(Hex.Hex hex)
    {
        hexMap.Add(hex);
        chunkMap.AssignHex(hex);
    }

    public void RemoveHex(HexVector position)
    {
        hexMap.Remove(position);
        chunkMap.RemoveHex(position);
    }

    public bool IsEmpty()
    {
        return hexMap.GetMap().Length <= 0;
    }
    
    public void HideChunks(HexVector[] positions)
    {
        chunkMap.HideChunks(positions);
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

    ~Layer()
    {
        chunkMap.Dispose();
    }
}