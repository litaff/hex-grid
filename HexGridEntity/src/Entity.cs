namespace HexGrid.Entity;

using Components;

public class Entity
{
    private readonly Dictionary<Type, Component> components;

    public IReadOnlyDictionary<Type, Component> Components => components;

    public bool Enabled { get; private set; }

    public Entity()
    {
        components = [];
    }
    
    public void AddComponent(Component component)
    {
        components[component.GetType()] = component;
    }

    public void RemoveComponent(Component component)
    {
        components.Remove(component.GetType());
    }

    public T? GetComponent<T>() where T : Component
    {
        return (T?)components.GetValueOrDefault(typeof(T));
    }

    public virtual void Enable()
    {
        Enabled = true;
        foreach (var component in components.Values)
        {
            component.Enable();
        }
    }

    public virtual void Disable()
    {
        Enabled = false;
        foreach (var component in components.Values)
        {
            component.Disable();
        }
    }
}