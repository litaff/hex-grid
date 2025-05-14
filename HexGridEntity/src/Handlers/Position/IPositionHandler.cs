namespace HexGrid.Entity.Handlers.Position;

using Providers;
using Map.Vector;

/// <summary>
/// This provider should assume that the object being moved is already on the passed position.
/// Meaning that when getting the height of the hex, the object is included.
/// </summary>
public interface IPositionHandler
{
    public void TranslateTo(HexVector position);
    public void Enable(IHexStateProvider provider);
    public void Disable();
}