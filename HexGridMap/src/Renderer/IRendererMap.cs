namespace HexGrid.Map.Renderer;

using Hex;
using Vector;

public interface IRendererMap
{
    public IReadOnlyDictionary<int, IRenderer> Renderers { get; }
    
    public void AddHex(Hex hex);
    public void AddTo(Hex[] hexes, HexVector position);
    public void RemoveHex(HexVector position);
    public void AddRenderer(IRenderer renderer);
    public void RemoveRenderer(HexVector position);
    public void Show();
    public void Show(HexVector[] rendererPositions);
    public IRenderer? FromHexPosition(HexVector position);
    public void Dispose();
}