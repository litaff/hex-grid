namespace HexGrid.Entity.Managers;

using System.Collections.Generic;
using Map.Hex;
using Map.Vector;
using Providers;
using Properties = Map.Properties;

public class EntityLayerManager : IHexStateProvider, IEntityLayerManager
{
    private readonly Dictionary<int, EntityStack> stacks;
    private readonly IHexProvider hexProvider;
    private readonly IEntityManager manager;

    public IReadOnlyDictionary<int, EntityStack> Stacks => stacks;
    public int Layer { get; }

    public EntityLayerManager(int layer, IHexProvider hexProvider, IEntityManager manager)
    {
        Layer = layer;
        this.hexProvider = hexProvider;
        this.manager = manager;
        stacks = new Dictionary<int, EntityStack>();
    }
    
    public void Add(Entity entity)
    {
        AddToStack(entity);
    }
    
    public void Remove(Entity entity)
    {
        RemoveFromStack(entity, entity.GridPosition);
    }

    public void UpdatePosition(Entity entity, HexVector previousPosition)
    {
        RemoveFromStack(entity, previousPosition);
        AddToStack(entity);
    }

    public void ChangeLayer(Entity entity, int relativeLayerIndex)
    {
        RemoveFromStack(entity, entity.GridPosition);
        // This will remove the object at its current position. This isn't a problem,
        // because we add this object to a different layer. 
        manager.Remove(entity, Layer); 
        manager.Add(entity, Layer + relativeLayerIndex);
    }
    
    public float GetHexHeight(HexVector position, List<Entity>? exclude = null)
    {
        var height = Layer * Properties.LayerHeight;
        
        height += hexProvider.GetHex(position)?.Height ?? 0;

        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return height;
        
        height += exclude != null ? stack.GetStackHeight(exclude) : stack.GetStackHeight();
        
        return height;
    }
    
    public bool Contains<T>(HexVector position)
    {
        return stacks.TryGetValue(position.GetHashCode(), out var stack) && stack.Contains<T>();
    }

    public bool IsBlocked(HexVector position, HexVector direction)
    {
        return stacks.TryGetValue(position.GetHashCode(), out var stack) && 
               stack.Entities.Any(entity => entity.BlockProvider.Blocks(direction));
    }
    
    public bool Exists(HexVector position)
    {
        return hexProvider.GetHex(position) is not null;
    }
    
    private void AddToStack(Entity entity)
    {
        var entityPosition = entity.GridPosition;
        if (stacks.ContainsKey(entityPosition.GetHashCode()))
        {
            stacks[entityPosition.GetHashCode()].Add(entity);
            return;
        }
        stacks.Add(entityPosition.GetHashCode(), new EntityStack(entity));
    }
    
    private void RemoveFromStack(Entity entity, HexVector position)
    {        
        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return;
        stack.Remove(entity);
        if (stack.IsEmpty)
        {
            stacks.Remove(position.GetHashCode());
        }
    }
}