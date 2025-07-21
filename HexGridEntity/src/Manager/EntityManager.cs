namespace HexGrid.Entity.Manager;

using Providers;
using Map.Vector;

public class EntityManager : IEntityManager
{
    private readonly IHexStateProvider hexStateProvider;

    public IEntityStorage EntityStorage { get; }

    public EntityManager(IHexStateProvider hexStateProvider, IEntityStorage entityStorage)
    {
        EntityStorage = entityStorage;
        this.hexStateProvider = hexStateProvider;
    }

    public void Add(Entity entity)
    {
        EntityStorage.Add(entity, entity.GridPosition);
        entity.RegisterHandlers();
        entity.Enable(this, hexStateProvider);
    }

    public void Remove(Entity entity)
    {
        entity.Disable();
        entity.UnregisterHandlers();
        EntityStorage.Remove(entity, entity.GridPosition);
    }

    public void MoveFrom(Entity entity, HexVector previousPosition)
    {
        EntityStorage.Remove(entity, previousPosition);
        EntityStorage.Add(entity, entity.GridPosition);
    }
}