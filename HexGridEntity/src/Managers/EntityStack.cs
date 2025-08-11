namespace HexGrid.Entity.Managers;

using System.Collections.Generic;
using System.Linq;
using Components;

public class EntityStack
{
    private readonly List<Entity> entities;

    public bool IsEmpty => entities.Count == 0;
    public IReadOnlyList<Entity> Entities => entities;

    public EntityStack()
    {
        entities = [];
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

    public List<T> GetComponents<T>(List<Entity>? exclude = null) where T : Component
    {
        var relevant = exclude is not null ? entities.Except(exclude) : entities;
        List<T> components = relevant.Select(entity => 
            entity.GetComponent<T>()).Where(component => 
            component is not null).ToList()!;
        return components;
    }

    public bool Contains<T>() where T : Entity
    {
        return entities.Any(entity => entity is T);
    }
}