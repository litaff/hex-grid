namespace HexGrid.Entity.Components.Blocker;

using Map.Vector;

public class Blocker : Component
{
    private readonly List<HexVector> blockedDirections;

    public IReadOnlyList<HexVector> BlockedDirections => blockedDirections;
    
    public Blocker(Entity entity, List<HexVector> blockedDirections) : base(entity)
    {
        this.blockedDirections = blockedDirections;
    }

    public bool Blocks(HexVector direction)
    {
        return Enabled && BlockedDirections.Contains(direction);
    }
}