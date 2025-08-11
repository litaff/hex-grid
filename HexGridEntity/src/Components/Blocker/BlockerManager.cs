namespace HexGrid.Entity.Components.Blocker;

using Managers;
using Map.Vector;

public class BlockerManager : IBlockerManager
{
    private readonly IEntityManager entityManager;

    public BlockerManager(IEntityManager entityManager)
    {
        this.entityManager = entityManager;
    }
    
    public bool IsBlocked(HexVector position, HexVector direction)
    {
        return entityManager.Stacks.TryGetValue(position.GetHashCode(), out var stack) && 
               stack.Entities.Any(entity => IsEntityBlockingDirection(entity, direction));
    }
    
    private bool IsEntityBlockingDirection(Entity entity, HexVector direction)
    {
        var blocker = entity.GetComponent<Blocker>();
        return blocker is not null && blocker.Blocks(direction);
    }
}