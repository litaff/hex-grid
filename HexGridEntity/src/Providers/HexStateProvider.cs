namespace HexGrid.Entity.Providers;

using Godot;
using Manager;
using Map.Hex;
using Map.Vector;

public class HexStateProvider : IHexStateProvider
{
    private readonly IHexProvider hexProvider;
    private readonly IEntityStorage entityStorage;

    public HexStateProvider(IHexProvider hexProvider, IEntityStorage entityStorage)
    {
        this.hexProvider = hexProvider;
        this.entityStorage = entityStorage;
    }
    
    public float GetHexHeight(HexVector position, List<Entity>? exclude = null)
    {
        var height = 0f;
        
        height += hexProvider.Get(position)?.Height ?? 0;

        if (!entityStorage.Stacks.TryGetValue(position.GetHashCode(), out var stack)) return height;
        
        height += stack.GetStackHeight(exclude);
        
        return height;
    }
    
    public bool Contains<T>(HexVector position) where T : Entity
    {
        return entityStorage.Stacks.TryGetValue(position.GetHashCode(), out var stack) && stack.Contains<T>();
    }

    // TODO: Prolly will be changed as a block provider won't be necessary for each entity.
    public bool IsBlocked(HexVector position, HexVector direction)
    {
        return entityStorage.Stacks.TryGetValue(position.GetHashCode(), out var stack) && 
               stack.Entities.Any(entity => entity.BlockProvider.Blocks(direction));
    }
    
    public bool Exists(HexVector position)
    {
        return hexProvider.Get(position) is not null;
    }

    public bool Exists(HexVector position, int stepRange)
    {
        return GetExistingHexes(position, stepRange).Count != 0;
    }

    public HexVector GetClosest(HexVector position, int stepRange)
    {
        var hexes = GetExistingHexes(position, stepRange);
        var closest = hexes.FirstOrDefault();
        return closest?.Position ?? position;
    }

    public bool TryGetClosest(HexVector position, int stepRange, out HexVector closest)
    {
        closest = position;
        var hexes = GetExistingHexes(position, stepRange);
        var closestHex = hexes.FirstOrDefault();
        if (closestHex is null) return false;
        closest = closestHex.Position;
        return true;
    }

    private List<Hex> GetExistingHexes(HexVector position, int stepRange)
    {
        var positions = GetPossiblePositions(position, stepRange);
        positions.Sort((a, b) =>
        {
            var aDiff = Mathf.Abs(position.H - a.H);
            var bDiff = Mathf.Abs(position.H - b.H);
            return aDiff - bDiff;
        });
        var hexes = positions.Select(pos => 
            hexProvider.Get(pos)).Where(hex => hex is not null).ToList();
        return hexes!;
    }

    private List<HexVector> GetPossiblePositions(HexVector position, int stepRange)
    {
        var positions = new List<HexVector>();
        for (var i = position.H - stepRange; i <= position.H + stepRange; i++)
        {
            var possible = position;
            possible.H = i;
            positions.Add(possible);
        }

        return positions;
    }
}