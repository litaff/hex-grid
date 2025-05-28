namespace HexGrid.Map.Tests.Renderer.Chunk;

using HexGrid.Map.Hex;
using HexGrid.Map.Renderer.Chunk;
using HexGrid.Map.Vector;

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
        chunk = new Chunk(position, verticalOffset, null, null);
    }

    [Test]
    public void Add_DoesNotAddHex_AtOccupiedPosition()
    {
        Assert.That(chunk.Hexes, Does.Not.Contain(hex));
        chunk.Add(hex);
        Assert.That(chunk.Hexes, Does.Contain(hex));
        var dupHex = new Hex(hexPosition, new Properties(), new MeshData());
        Assert.That(chunk.Hexes, Does.Not.Contain(dupHex));
        
        chunk.Add(dupHex);
        
        Assert.That(chunk.Hexes, Does.Not.Contain(dupHex));
    }

    [Test]
    public void Add_AddsUniqueHex()
    {
        Assert.That(chunk.Hexes, Does.Not.Contain(hex));
        
        chunk.Add(hex);
        
        Assert.That(chunk.Hexes, Does.Contain(hex));
    }

    [Test]
    public void Remove_DoesNotThrow_IfNoHex_AtPosition()
    {
        Assert.That(chunk.Hexes, Does.Not.Contain(hex));
        
        Assert.DoesNotThrow(() => chunk.Remove(hex.Position));
    }
    
    [Test]
    public void Remove_RemovesHexAtPosition()
    {
        chunk.Add(hex);
        Assert.That(chunk.Hexes, Does.Contain(hex));

        chunk.Remove(hex.Position);
        
        Assert.That(chunk.Hexes, Does.Not.Contain(hex));
    }

    [Test]
    public void Overlaps_ReturnsTrue_WhenRenderersHaveTheSamePosition()
    {
        var overlap = new Chunk(position, verticalOffset);
        
        Assert.Multiple(() =>
        {
            Assert.That(overlap.Position, Is.EqualTo(chunk.Position));
            Assert.That(chunk.Overlaps(overlap), Is.True);
        });
    }
    
    [Test]
    public void Overlaps_ReturnsFalse_WhenRenderersHaveDifferentPositions()
    {
        var overlapPosition = position + HexVector.North;
        var overlap = new Chunk(overlapPosition, verticalOffset);
        
        Assert.Multiple(() =>
        {
            Assert.That(overlap.Position, Is.Not.EqualTo(chunk.Position));
            Assert.That(chunk.Overlaps(overlap), Is.False);
        });
    }
}