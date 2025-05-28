namespace HexGrid.Map.Tests.Renderer.Chunk;

using Map.Hex;
using Map.Renderer.Chunk;
using Map.Vector;
using Properties = Properties;

[TestFixture]
public class ChunkMapTests
{
    private ChunkMap map;
    private Properties properties;

    [SetUp]
    public void SetUp()
    {
        properties = new Properties(1, 1, 1);
        map = new ChunkMap(1);
    }

    [Test]
    public void AddHex_CreatesAChunk_WhenNoChunkAtPosition()
    {
        var position = HexVector.Zero;
        var hex = new Hex(position, new Map.Hex.Properties(), new MeshData());
        Assert.That(map.Renderers, Is.Empty);
        
        map.AddHex(hex);
        
        Assert.That(map.Renderers, Is.Not.Empty);
    }
    
    [Test]
    public void AddHex_DoesNotCreateAChunk_WhenChunkAtPosition()
    {
        var position = HexVector.Zero;
        var hex = new Hex(position, new Map.Hex.Properties(), new MeshData());
        Assert.That(map.Renderers, Is.Empty);
        map.AddHex(hex);
        Assert.That(map.Renderers, Is.Not.Empty);
        Assert.That(map.Renderers, Has.Count.EqualTo(1));
        position += HexVector.North;
        hex = new Hex(position, new Map.Hex.Properties(), new MeshData());
        
        map.AddHex(hex);
        
        Assert.That(map.Renderers, Has.Count.EqualTo(1));
    }

    [Test]
    public void RemoveHex_DoesNotThrow_WhenNothingToRemove()
    {
        Assert.DoesNotThrow(() => map.RemoveHex(HexVector.Zero));
    }

    [Test]
    public void RemoveHex_RemovedChunk_IfItsEmpty()
    {
        var position = HexVector.Zero;
        var hex = new Hex(position, new Map.Hex.Properties(), new MeshData());
        Assert.That(map.Renderers, Is.Empty);
        map.AddHex(hex);
        Assert.That(map.Renderers, Is.Not.Empty);
        
        map.RemoveHex(position);
        
        Assert.That(map.Renderers, Is.Empty);
    }
}