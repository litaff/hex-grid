namespace HexGrid.Entity.Managers;

using Components.Transform;
using Map.Vector;

public class EntityManager : IEntityManager
{
    private readonly Dictionary<int, EntityStack> stacks = new();

    public IReadOnlyDictionary<int, EntityStack> Stacks => stacks;

    public void Add(Entity entity)
    {
        var transform = entity.GetComponent<Transform>();
        if (transform is null) return;
        var position = transform.Position.Current;
        transform.OnUpdated += OnEntityPositionChangedHandler;
        AddToStack(entity, position);
    }

    public void Remove(Entity entity)
    {
        var transform = entity.GetComponent<Transform>();
        if (transform is null) return;
        var position = transform.Position.Current;
        if (!RemoveFromStack(entity, position)) return;
        transform.OnUpdated -= OnEntityPositionChangedHandler;
    }
    
    public bool Contains<T>(HexVector position) where T : Entity
    {
        return Stacks.TryGetValue(position.GetHashCode(), out var stack) && stack.Contains<T>();
    }
    
    private void OnEntityPositionChangedHandler(Transform transform)
    {
        var previousPosition = transform.Position.Previous;
        RemoveFromStack(transform.Entity, previousPosition);

        var currentPosition = transform.Position.Current;
        AddToStack(transform.Entity, currentPosition);
    }

    private void AddToStack(Entity entity, HexVector position)
    {
        if (stacks.ContainsKey(position.GetHashCode()))
        {
            stacks[position.GetHashCode()].Add(entity);
            return;
        }
        stacks.Add(position.GetHashCode(), new EntityStack(entity));
    }

    private bool RemoveFromStack(Entity entity, HexVector position)
    {
        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return false;
        stack.Remove(entity);
        if (stack.IsEmpty)
        {
            stacks.Remove(position.GetHashCode());
        }

        return true;
    }
}