namespace HexGrid.Map.Renderer;

public interface IRendererMapFactory
{
    public IRendererMap New(int index);
}