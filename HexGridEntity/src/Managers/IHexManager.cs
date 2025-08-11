namespace HexGrid.Entity.Managers;

using Map.Vector;

public interface IHexManager
{
    public float GetHeight(HexVector position);
    public bool Exists(HexVector position);
    public bool Exists(HexVector position, int stepRange);
    public HexVector GetClosest(HexVector position, int stepRange);
    public bool TryGetClosest(HexVector position, int stepRange, out HexVector closest);
}