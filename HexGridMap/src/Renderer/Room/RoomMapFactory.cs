namespace HexGrid.Map.Renderer.Room;

using Godot;

public class RoomMapFactory : IRendererMapFactory
{
    private readonly MeshLibrary? meshLibrary;
    private readonly Node3D? parent;
    
    public RoomMapFactory(MeshLibrary? meshLibrary = null, Node3D? parent = null)
    {
        this.meshLibrary = meshLibrary;
        this.parent = parent;
    }
    
    public IRendererMap New(int index)
    {
        return new RoomMap(meshLibrary, parent);
    }
}