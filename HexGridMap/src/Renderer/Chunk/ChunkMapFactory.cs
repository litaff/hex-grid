namespace HexGrid.Map.Renderer.Chunk;

using Godot;

public class ChunkMapFactory : IRendererMapFactory
{
    private readonly MeshLibrary? library;
    private readonly World3D? scenario;

    public ChunkMapFactory(MeshLibrary? library = null, World3D? scenario = null)
    {
        this.library = library;
        this.scenario = scenario;
    }
    
    public IRendererMap New(int index)
    {
        return new ChunkMap(index * Properties.LayerHeight, library, scenario);
    }
}