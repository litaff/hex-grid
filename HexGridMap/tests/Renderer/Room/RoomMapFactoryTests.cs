namespace HexGrid.Map.Tests.Renderer.Room;

using Map.Renderer.Room;

[TestFixture]
public class RoomMapFactoryTests
{
    private RoomMapFactory factory;
    
    [SetUp]
    public void SetUp()
    {
        factory = new RoomMapFactory();
    }

    [Test]
    public void New_ReturnsChunkMap()
    {
        const int index = 0;

        var map = factory.New(index);
        
        Assert.That(map, Is.Not.Null);
        Assert.That(map, Is.TypeOf<RoomMap>());
    }
}