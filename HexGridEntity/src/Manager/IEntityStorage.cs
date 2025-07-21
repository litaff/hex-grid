namespace HexGrid.Entity.Manager;

using Map.Vector;

public interface IEntityStorage
{
    public IReadOnlyDictionary<int, EntityStack> Stacks { get; }
    
    public void Add(Entity entity, HexVector position);
    public void Remove(Entity entity, HexVector position);
}