namespace HexGrid.Map.Renderer.Room;

using Godot;
using Hex;
using Vector;

public class RoomMap : IRendererMap
{
    private readonly Dictionary<int, Room> renderers;
    private readonly Node3D? mapParent;
    private readonly MeshLibrary? meshLibrary;

    public IReadOnlyDictionary<int, IRenderer> Renderers => renderers.ToDictionary(pair => pair.Key, IRenderer (pair) => pair.Value);

    private HexVector DefaultRenderer => HexVector.Zero;
    
    public RoomMap(MeshLibrary? meshLibrary = null, Node3D? mapParent = null)
    {
        renderers = new Dictionary<int, Room>();
        this.mapParent = mapParent;
        this.meshLibrary = meshLibrary;
    }

    public void AddTo(Hex[] hexes, HexVector position)
    {
        if (!renderers.TryGetValue(position.GetHashCode(), out var room))
        {
            room = new Room(position, meshLibrary, mapParent);
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

    public void Show(HexVector[] rendererPositions)
    {
        foreach (var renderer in renderers.Values)
        {
            if (rendererPositions.Contains(renderer.Position))
            {
                renderer.Display();
                continue;
            }
            renderer.Hide();
        }
    }

    public void Show()
    {
        foreach (var renderer in renderers.Values)
        {
            renderer.Display();
        }
    }

    public IRenderer? FromHexPosition(HexVector position)
    {
        return renderers.Values.FirstOrDefault(renderer => renderer.Hexes.Any(hex => hex.Position == position));
    }

    public void Dispose()
    {
        foreach (var renderer in renderers.Values)
        {
            renderer.Dispose();
        }
    }
}