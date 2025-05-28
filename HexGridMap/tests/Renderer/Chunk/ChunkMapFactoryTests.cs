namespace HexGrid.Map.Tests.Renderer.Chunk;

using Map.Renderer.Chunk;

[TestFixture]
public class ChunkMapFactoryTests
{
    private ChunkMapFactory factory;
    private Properties properties;

    [SetUp]
    public void SetUp()
    {
        properties = new Properties(1, 1, 1);
        factory = new ChunkMapFactory();
    }

    [Test]
    public void New_ReturnsChunkMap()
    {
        const int index = 0;

        var map = factory.New(index);
        
        Assert.That(map, Is.Not.Null);
        Assert.That(map, Is.TypeOf<ChunkMap>());
    }
}