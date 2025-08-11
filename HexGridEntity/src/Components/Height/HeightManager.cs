namespace HexGrid.Entity.Components.Height;

using Map.Vector;
using Managers;

public class HeightManager : IHeightManager
{
    private readonly IEntityManager entityManager;

    public HeightManager(IEntityManager entityManager)
    {
        this.entityManager = entityManager;
    }
    
    public float GetHeight(HexVector position, List<Entity>? exclude = null)
    {
        var height = 0f;

        if (!entityManager.Stacks.TryGetValue(position.GetHashCode(), out var stack)) return height;
        
        height += stack.GetComponents<Height>(exclude).Sum(component => component.Value);
        
        return height;
    }
}