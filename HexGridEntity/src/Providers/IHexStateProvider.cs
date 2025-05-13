namespace HexGrid.Entity.Providers;

using Map.Vector;

public interface IHexStateProvider
{
    public float GetHexHeight(HexVector position, List<Entity>? exclude = null);
    public bool Contains<T>(HexVector position);
    public bool IsBlocked(HexVector position, HexVector direction);
    public bool Exists(HexVector position);
}