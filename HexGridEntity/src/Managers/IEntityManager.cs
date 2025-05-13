namespace HexGrid.Entity.Managers;

public interface IEntityManager
{
    public void Add(Entity entity, int layer);
    public void Remove(Entity entity, int layer);
}