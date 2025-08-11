namespace HexGrid.Map.Tests.Renderer.Room;

using Map.Hex;
using Map.Renderer.Room;
using Map.Vector;

[TestFixture]
public class RoomTests
{
    private Room room;
    private HexVector roomPosition;
    private HexGrid.Map.Properties properties;
    private HexVectorConverter converter;
    
    [SetUp]
    public void SetUp()
    {
        properties = new HexGrid.Map.Properties(1, 1);
        converter = new HexVectorConverter(properties);
        roomPosition = HexVector.Zero;
        room = new Room(roomPosition, converter);
    }

    [Test]
    public void Overlaps_ReturnsTrue_IfAnyHexesOccupyTheSamePosition()
    {
        var overlap = new Room(roomPosition, converter);
        var hex = new Hex(HexVector.Zero, new Properties(), new MeshData());
        room.Add(hex);
        overlap.Add(hex);
        
        Assert.That(room.Overlaps(overlap), Is.True);
    }

    [Test]
    public void Overlaps_ReturnsFalse_IfNoHexesOccupyTheSamePosition()
    {
        var overlap = new Room(roomPosition, converter);
        var hex = new Hex(HexVector.Zero, new Properties(), new MeshData());
        room.Add(hex);
        hex = new Hex(HexVector.Forward, new Properties(), new MeshData());
        overlap.Add(hex);
        
        Assert.That(room.Overlaps(overlap), Is.False);
    }
}