namespace HexGrid.Map.Renderer.Room;

using Godot;
using Hex;
using Vector;

public class RoomMap : IRendererMap
{
    private readonly Dictionary<int, Room> renderers;
    private readonly Node3D? mapParent;
    private readonly MeshLibrary? meshLibrary;
    private readonly HexVectorConverter converter;

    public IReadOnlyDictionary<int, IRenderer> Renderers => renderers.ToDictionary(pair => pair.Key, IRenderer (pair) => pair.Value);

    private HexVector DefaultRenderer => HexVector.Zero;
    
    public RoomMap(HexVectorConverter converter, MeshLibrary? meshLibrary = null, Node3D? mapParent = null)
    {
        this.converter = converter;
        renderers = new Dictionary<int, Room>();
        this.mapParent = mapParent;
        this.meshLibrary = meshLibrary;
    }

    public void AddTo(Hex[] hexes, HexVector position)
    {
        if (!renderers.TryGetValue(position.GetHashCode(), out var room))
        {
            room = new Room(position, converter, meshLibrary, mapParent);
            renderers.Add(position.GetHashCode(), room);
        }

        foreach (var hex in hexes)
        {
            room.Add(hex);
        }
        room.Update();
    }

    public void AddHex(Hex hex)
    {
        AddTo([hex], DefaultRenderer);
    }

    public void RemoveHex(HexVector position)
    {
        if (!renderers.TryGetValue(DefaultRenderer.GetHashCode(), out var room)) return;
        room.Remove(position);
        room.Update();
        if (room.Hexes.Count != 0) return;
        renderers.Remove(room.Position.GetHashCode());
        room.Dispose();
    }

    public void AddRenderer(IRenderer renderer)
    {
        if (renderer is not Room room) return;
        renderers.TryAdd(room.Position.GetHashCode(), room);
    }

    public void RemoveRenderer(HexVector position)
    {
        renderers.Remove(position.GetHashCode());
    }
    
    public IRenderer? FromHexPosition(HexVector position)
    {
        return renderers.Values.FirstOrDefault(renderer => renderer.Hexes.Any(hex => hex.Position == position));
    }

    public void Occlude(Predicate<HexVector> hexPositionPredicate)
    {
        GD.Print("TODO occlusion.");
    }

    public void Dispose()
    {
        foreach (var renderer in renderers.Values)
        {
            renderer.Dispose();
        }
    }
}