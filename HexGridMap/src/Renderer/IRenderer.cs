namespace HexGrid.Map.Renderer;

using Hex;
using Vector;

public interface IRenderer
{
    public HexVector Position { get; }
    public IReadOnlyList<Hex> Hexes { get; }
 
    public void Add(Hex hex);
    public void Remove(HexVector position);
    public void Display();
    public void Hide();
    public void Update();
    public bool Overlaps(IRenderer renderer);
    public void Dispose();
}