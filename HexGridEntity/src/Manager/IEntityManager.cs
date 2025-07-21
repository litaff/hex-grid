namespace HexGrid.Entity.Manager;

using Map.Vector;

public interface IEntityManager
{
    public IEntityStorage EntityStorage { get; }
    
    public void Add(Entity entity);
    public void Remove(Entity entity);
    public void MoveFrom(Entity entity, HexVector previousPosition);
}