namespace HexGrid.Map.Tests.Renderer.Room;

using Map.Hex;
using Map.Renderer.Room;
using Map.Vector;

[TestFixture]
public class RoomTests
{
    private Room room;
    private HexVector roomPosition;
    
    [SetUp]
    public void SetUp()
    {
        roomPosition = HexVector.Zero;
        room = new Room(roomPosition);
    }

    [Test]
    public void Overlaps_ReturnsTrue_IfAnyHexesOccupyTheSamePosition()
    {
        var overlap = new Room(roomPosition);
        var hex = new Hex(HexVector.Zero, new Properties(), new MeshData());
        room.Add(hex);
        overlap.Add(hex);
        
        Assert.That(room.Overlaps(overlap), Is.True);
    }

    [Test]
    public void Overlaps_ReturnsFalse_IfNoHexesOccupyTheSamePosition()
    {
        var overlap = new Room(roomPosition);
        var hex = new Hex(HexVector.Zero, new Properties(), new MeshData());
        room.Add(hex);
        hex = new Hex(HexVector.Forward, new Properties(), new MeshData());
        overlap.Add(hex);
        
        Assert.That(room.Overlaps(overlap), Is.False);
    }
}