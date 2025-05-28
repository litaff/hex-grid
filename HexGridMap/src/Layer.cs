namespace HexGrid.Map;

using Hex;
using Renderer;
using Vector;

public class Layer : IHexProvider
{
    private readonly HexMap hexMap;
    private readonly IRendererMap rendererMap;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hexMapData">Map data to be assigned to the hex map.</param>
    /// <param name="rendererMap">Map renderer with the <see cref="hexMapData"/> already assigned</param>
    public Layer(IHexMapData hexMapData, IRendererMap rendererMap)
    {
        hexMap = new HexMap(hexMapData);
        this.rendererMap = rendererMap;
    }

    public void AddMap(IHexMapData mapData, IRenderer renderer)
    {
        hexMap.Add(mapData);
        rendererMap.AddRenderer(renderer);
    }
    
    public Hex.Hex? GetHex(HexVector position)
    {
        return hexMap.Get(position);
    }
    
    public void AddHex(Hex.Hex hex)
    {
        hexMap.Add(hex);
        rendererMap.AddHex(hex);
    }

    public void RemoveHex(HexVector position)
    {
        hexMap.Remove(position);
        rendererMap.RemoveHex(position);
    }

    public bool IsEmpty()
    {
        return hexMap.GetMap().Length <= 0;
    }
    
    public void DisplayChunks(HexVector[] positions)
    {
        rendererMap.Show(positions);
    }

    public void Display()
    {
        rendererMap.Show();
    }
    
    public void Hide()
    {
        rendererMap.Show([]);
    }
    
    public void Dispose()
    {
        rendererMap.Dispose();
    }

    ~Layer()
    {
        rendererMap.Dispose();
    }
}