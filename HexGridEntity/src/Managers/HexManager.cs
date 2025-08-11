namespace HexGrid.Entity.Managers;

using Godot;
using Map.Hex;
using Map.Vector;

public class HexManager : IHexManager
{
    private readonly IHexProvider hexProvider;

    public HexManager(IHexProvider hexProvider)
    {
        this.hexProvider = hexProvider;
    }

    public float GetHeight(HexVector position)
    {
        return hexProvider.Get(position)?.Elevation ?? 0;
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