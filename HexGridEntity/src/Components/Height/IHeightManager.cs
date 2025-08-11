namespace HexGrid.Entity.Components.Height;

using Map.Vector;

public interface IHeightManager : IComponentManager
{
    public float GetHeight(HexVector position, List<Entity>? exclude = null);
}