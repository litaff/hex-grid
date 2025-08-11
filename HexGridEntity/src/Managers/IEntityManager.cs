namespace HexGrid.Entity.Managers;

using Map.Vector;

public interface IEntityManager
{
    public IReadOnlyDictionary<int, EntityStack> Stacks { get; }
    
    public void Add(Entity entity);
    public void Remove(Entity entity);
    public bool Contains<T>(HexVector position) where T : Entity;
}