namespace HexGrid.Entity.Providers;

using Map.Vector;

public interface IHexStateProvider
{
    public float GetHexHeight(HexVector position, List<Entity>? exclude = null);
    public bool Contains<T>(HexVector position) where T : Entity;
    public bool IsBlocked(HexVector position, HexVector direction);
    public bool Exists(HexVector position);
    public bool Exists(HexVector position, int stepRange);
    public HexVector GetClosest(HexVector position, int stepRange);
    public bool TryGetClosest(HexVector position, int stepRange, out HexVector closest);
}