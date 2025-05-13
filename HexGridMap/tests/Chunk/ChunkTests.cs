namespace HexGrid.Map.Tests.Chunk;

using Map.Chunk;
using Map.Hex;
using Map.Vector;

[TestFixture]
public class ChunkTests
{
    private Chunk chunk;
    private HexVector position;
    private float verticalOffset;
    
    private Hex hex;
    private HexVector hexPosition;
    
    [SetUp]
    public void SetUp()
    {
        hexPosition = HexVector.Zero;
        hex = new Hex(hexPosition, new Properties(), new MeshData());
        
        position = HexVector.Zero;
        verticalOffset = 0f;
        chunk = new Chunk(position, verticalOffset);
    }

    [Test]
    public void Add_DoesNotAddHex_AtOccupiedPosition()
    {
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(hex));
        chunk.Add(hex);
        Assert.That(chunk.AssignedHexes, Does.Contain(hex));
        var dupHex = new Hex(hexPosition, new Properties(), new MeshData());
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(dupHex));
        
        chunk.Add(dupHex);
        
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(dupHex));
    }

    [Test]
    public void Add_AddsUniqueHex()
    {
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(hex));
        
        chunk.Add(hex);
        
        Assert.That(chunk.AssignedHexes, Does.Contain(hex));
    }

    [Test]
    public void Remove_DoesNotThrow_IfNoHex_AtPosition()
    {
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(hex));
        
        Assert.DoesNotThrow(() => chunk.Remove(hex.Position));
    }
    
    [Test]
    public void Remove_RemovesHexAtPosition()
    {
        chunk.Add(hex);
        Assert.That(chunk.AssignedHexes, Does.Contain(hex));

        chunk.Remove(hex.Position);
        
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(hex));
    }
}