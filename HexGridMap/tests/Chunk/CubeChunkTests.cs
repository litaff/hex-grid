namespace HexGridMap.Tests.Chunk;

using global::HexGridMap.Chunk;
using global::HexGridMap.Hex;
using global::HexGridMap.Vector;

[TestFixture]
public class CubeChunkTests
{
    private CubeChunk chunk;
    private CubeHexVector position;
    private float verticalOffset;
    
    private CubeHex hex;
    private CubeHexVector hexPosition;
    
    [SetUp]
    public void SetUp()
    {
        hexPosition = CubeHexVector.Zero;
        hex = new CubeHex(hexPosition, new HexProperties(), new HexMeshData());
        
        position = CubeHexVector.Zero;
        verticalOffset = 0f;
        chunk = new CubeChunk(position, verticalOffset);
    }

    [Test]
    public void Add_DoesNotAddHex_AtOccupiedPosition()
    {
        Assert.That(chunk.AssignedHexes, Does.Not.Contain(hex));
        chunk.Add(hex);
        Assert.That(chunk.AssignedHexes, Does.Contain(hex));
        var dupHex = new CubeHex(hexPosition, new HexProperties(), new HexMeshData());
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