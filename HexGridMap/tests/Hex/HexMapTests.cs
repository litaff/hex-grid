namespace HexGridMap.Tests.Hex;

using global::HexGridMap.Hex;
using global::HexGridMap.Vector;
using Moq;

[TestFixture]
public class HexMapTests
{
    private Mock<IHexData> mockHexData;
    private HexMap map;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new HexGridProperties(1, 1, 1);
        mockHexData = new Mock<IHexData>();
        mockHexData.Setup(mock => mock.Deserialize()).Returns(new Dictionary<int, CubeHex>());
        map = new HexMap(mockHexData.Object);
    }

    [Test]
    public void Add_AddsHex_AtPosition()
    {
        var position = CubeHexVector.Zero;
        var hex = new CubeHex(position, new HexProperties(), new HexMeshData());
        var stored = map.Get(position);
        Assert.That(stored, Is.Null);
        
        map.Add(hex);
        
        stored = map.Get(position);
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored, Is.EqualTo(hex));
    }

    [Test]
    public void Add_DoesNotThrow_WhenAddingSamePosition_Twice()
    {
        var hex = new CubeHex(0,0, new HexProperties(), new HexMeshData());
        map.Add(hex);

        Assert.DoesNotThrow(() => map.Add(hex));
    }

    [Test]
    public void Remove_DoesNotThrow_IfNothingAtPosition()
    {
        var position = CubeHexVector.Zero;

        Assert.DoesNotThrow(() => map.Remove(position));
    }
    
    [Test]
    public void Remove_RemovesHex_AtPosition()
    {
        var position = CubeHexVector.Zero;
        var hex = new CubeHex(position, new HexProperties(), new HexMeshData());
        map.Add(hex);
        var stored = map.Get(position);
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored, Is.EqualTo(hex));
        
        map.Remove(position);
        
        stored = map.Get(position);
        Assert.That(stored, Is.Null);
    }

    [Test]
    public void Get_ReturnsNull_IfDoesNotExists()
    {
        var position = CubeHexVector.Zero;

        var stored = map.Get(position);
        
        Assert.That(stored, Is.Null);
    }
    
    [Test]
    public void Get_ReturnsHex_IfExists()
    {
        var position = CubeHexVector.Zero;
        var hex = new CubeHex(position, new HexProperties(), new HexMeshData());
        map.Add(hex);

        var stored = map.Get(position);
        
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored, Is.EqualTo(hex));
    }

    [Test]
    public void GetMap_ReturnsNoHexes_IfEmpty()
    {
        var hexes = map.GetMap();
        
        Assert.That(hexes, Is.Empty);
    }
    
    [Test]
    public void GetMap_ReturnsAllHexes_IfNotEmpty()
    {
        var hex = new CubeHex(0,0, new HexProperties(), new HexMeshData());
        map.Add(hex);
        
        var hexes = map.GetMap();
        
        Assert.That(hexes, Is.Not.Empty);
        Assert.That(hexes.Length, Is.EqualTo(1));
    }
}