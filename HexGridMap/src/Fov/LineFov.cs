namespace HexGridMap.Fov;

using Vector;

/// <summary>
/// Provides fov data by drawing a line to an outer hex and checking if any passed hex is an occluder.
/// </summary>
public class LineFov : IFovProvider
{
    private readonly IHexProvider hexProvider;
    
    public LineFov(IHexProvider hexProvider)
    {
        this.hexProvider = hexProvider;
    }
    
    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius)
    {
        var visiblePositions = new Dictionary<int, CubeHexVector>();
        var edge = origin.GetRing(radius);
        foreach (var edgePoint in edge)
        {
            var visionLine = origin.LineTo(edgePoint);
            var hexes = visionLine.Select(position => hexProvider.GetHex(position)).Where(hex => hex != null);
            var positions = hexes.TakeWhile(hex => !hex!.IsOccluder).Select(hex => hex!.Position);
            foreach (var position in positions)
            {
                visiblePositions.TryAdd(position.GetHashCode(), position);
            }
        }
        return visiblePositions.Values.ToArray();
    }
}