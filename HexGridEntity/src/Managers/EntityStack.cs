namespace HexGrid.Entity.Managers;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Keeps the order of entry. 
/// </summary>
public readonly struct EntityStack
{
    private readonly List<Entity> entities;

    public bool IsEmpty => entities.Count == 0;
    public IReadOnlyList<Entity> Entities => entities;

    public EntityStack()
    {
        entities = new List<Entity>();
    }
    
    public EntityStack(Entity initialEntity)
    {
        entities = [initialEntity];
    }

    public void Add(Entity entity)
    {
        if (entities.Contains(entity)) return;
        entities.Add(entity);
    }

    public void Remove(Entity entity)
    {
        entities.Remove(entity);
    }

    public float GetStackHeight()
    {
        return entities.Sum(entity => entity.HeightData.Height);
    }

    public float GetStackHeight(List<Entity> exclude)
    {
        return entities.Except(exclude).Sum(entity => entity.HeightData.Height);
    }

    public bool Contains<T>()
    {
        return entities.Any(entity => entity is T);
    }
}