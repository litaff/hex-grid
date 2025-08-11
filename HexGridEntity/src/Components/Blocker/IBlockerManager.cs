namespace HexGrid.Entity.Components.Blocker;

using Map.Vector;

public interface IBlockerManager : IComponentManager
{
    public bool IsBlocked(HexVector position, HexVector direction);
}