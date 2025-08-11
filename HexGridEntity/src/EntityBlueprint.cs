namespace HexGrid.Entity;

using Components;

public class EntityBlueprint
{
    private readonly Dictionary<Type, IComponentDescriptor> descriptors = [];

    public IReadOnlyDictionary<Type, IComponentDescriptor> Descriptors => descriptors;

    public void Add<T>(T descriptor) where T : IComponentDescriptor
    {
        descriptors[typeof(T)] = descriptor;
    }

    public T Get<T>() where T : IComponentDescriptor
    {
        var descriptor = descriptors.GetValueOrDefault(typeof(T));
        if (descriptor is null)
        {
            throw new ArgumentNullException($"[EntityBlueprint] Tried to get descriptor of type {typeof(T)}, but no was found!");
        }
        return (T)descriptor;
    }
}