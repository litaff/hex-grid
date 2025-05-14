namespace HexGrid.Entity.Handlers.Position;

public interface IUpdateablePositionHandler : IPositionHandler
{
    public bool TranslationComplete { get; }
    public void Update(double delta);
}