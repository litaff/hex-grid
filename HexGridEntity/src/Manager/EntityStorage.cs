namespace HexGrid.Entity.Manager;

using Map.Vector;

public class EntityStorage : IEntityStorage
{
    private readonly Dictionary<int, EntityStack> stacks = new();

    public IReadOnlyDictionary<int, EntityStack> Stacks => stacks;

    public void Add(Entity entity, HexVector position)
    {
        if (stacks.ContainsKey(position.GetHashCode()))
        {
            stacks[position.GetHashCode()].Add(entity);
            return;
        }
        stacks.Add(position.GetHashCode(), new EntityStack(entity));
    }

    public void Remove(Entity entity, HexVector position)
    {
        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return;
        stack.Remove(entity);
        if (stack.IsEmpty)
        {
            stacks.Remove(position.GetHashCode());
        }
    }
}