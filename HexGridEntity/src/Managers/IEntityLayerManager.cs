namespace HexGrid.Entity.Managers;

using Map.Vector;

public interface IEntityLayerManager
{
    public void UpdatePosition(Entity entity, HexVector previousPosition);
    public void ChangeLayer(Entity entity, int relativeLayerIndex);
}