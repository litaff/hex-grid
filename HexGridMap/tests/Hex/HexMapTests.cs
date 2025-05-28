namespace HexGrid.Map.Tests.Hex;

using Map.Hex;
using Map.Vector;
using Moq;

[TestFixture]
public class HexMapTests
{
    private Mock<IHexMapData> mockHexMapData;
    private HexMap map;
    
    [SetUp]
    public void SetUp()
    {
        var properties = new Map.Properties(1, 1, 1);
        mockHexMapData = new Mock<IHexMapData>();
        mockHexMapData.Setup(mock => mock.Map).Returns(new Dictionary<int, Hex>());
        map = new HexMap(mockHexMapData.Object);
    }

    [Test]
    public void Add_DeserializesMapData()
    {
        var mockData = new Mock<IHexMapData>();
        mockData.Setup(mock => mock.Map).Returns(new Dictionary<int, Hex>());
        
        map.Add(mockData.Object);
        
        mockData.Verify(data => data.Deserialize(), Times.Once);
    }

    [Test]
    public void Add_SerializesPrimaryData()
    {
        var mockData = new Mock<IHexMapData>();
        mockData.Setup(mock => mock.Map).Returns(new Dictionary<int, Hex>());
        
        map.Add(mockData.Object);
        
        mockHexMapData.Verify(data => data.Serialize(), Times.Once);
    }

    [Test]
    public void Add_AddsHexes_FromPassedData()
    {
        var mockData = new Mock<IHexMapData>();
        var hex = new Hex(HexVector.Zero, new Properties(), new MeshData());
        mockData.Setup(mock => mock.Map).Returns(new Dictionary<int, Hex>
        {
            { hex.Position.GetHashCode(), hex }
        });
        Assert.That(map.GetMap(), Is.Empty);
        
        map.Add(mockData.Object);
        
        Assert.That(map.GetMap(), Is.Not.Empty);
    }
    
    [Test]
    public void Add_AddsHex_AtPosition()
    {
        var position = HexVector.Zero;
        var hex = new Hex(position, new Properties(), new MeshData());
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
        var hex = new Hex(0,0, new Properties(), new MeshData());
        map.Add(hex);

        Assert.DoesNotThrow(() => map.Add(hex));
    }

    [Test]
    public void Remove_DoesNotThrow_IfNothingAtPosition()
    {
        var position = HexVector.Zero;

        Assert.DoesNotThrow(() => map.Remove(position));
    }
    
    [Test]
    public void Remove_RemovesHex_AtPosition()
    {
        var position = HexVector.Zero;
        var hex = new Hex(position, new Properties(), new MeshData());
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
        var position = HexVector.Zero;

        var stored = map.Get(position);
        
        Assert.That(stored, Is.Null);
    }
    
    [Test]
    public void Get_ReturnsHex_IfExists()
    {
        var position = HexVector.Zero;
        var hex = new Hex(position, new Properties(), new MeshData());
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
        var hex = new Hex(0,0, new Properties(), new MeshData());
        map.Add(hex);
        
        var hexes = map.GetMap();
        
        Assert.That(hexes, Is.Not.Empty);
        Assert.That(hexes.Length, Is.EqualTo(1));
    }
}