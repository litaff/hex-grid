namespace HexGrid.Entity.Components;

public class ManagerRegistry
{
    private readonly Dictionary<Type, IComponentManager> managers;

    public IReadOnlyDictionary<Type, IComponentManager> Managers => managers;

    public ManagerRegistry()
    {
        managers = [];
    }
    
    public void Add(IComponentManager manager)
    {
        if (managers.Any(man => man.GetType() == manager.GetType())) return;
        managers[manager.GetType()] = manager;
    }

    public T Get<T>() where T : IComponentManager
    {
        var manager = managers.GetValueOrDefault(typeof(T));
        if (manager is null)
        {
            throw new ArgumentException($"[ComponentRegistry] Manager of type {typeof(T)} not found!");
        }
        return (T)manager;
    }
}