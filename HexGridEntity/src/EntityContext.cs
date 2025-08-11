namespace HexGrid.Entity;

using Components;
using Managers;
using Map.Vector;

public class EntityContext
{
    public ManagerRegistry ManagerRegistry { get; }
    public IHexManager HexManager { get; }
    public IEntityManager EntityManager { get; }
    public HexVectorConverter Converter { get; }

    public EntityContext(IHexManager hexManager, IEntityManager entityManager, ManagerRegistry managerRegistry, 
        HexVectorConverter converter)
    {
        EntityManager = entityManager;
        HexManager = hexManager;
        ManagerRegistry = managerRegistry;
        Converter = converter;
    }
}