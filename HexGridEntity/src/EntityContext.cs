namespace HexGrid.Entity;

using Components;
using Managers;

public class EntityContext
{
    public ManagerRegistry ManagerRegistry { get; }
    public IHexManager HexManager { get; }
    public IEntityManager EntityManager { get; }

    public EntityContext(IHexManager hexManager, IEntityManager entityManager, ManagerRegistry managerRegistry)
    {
        EntityManager = entityManager;
        HexManager = hexManager;
        ManagerRegistry = managerRegistry;
    }
}