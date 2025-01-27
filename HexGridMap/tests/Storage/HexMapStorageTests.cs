namespace HexGridMap.Tests.Storage;

using global::HexGridMap.Hex;
using global::HexGridMap.Storage;
using global::HexGridMap.Vector;
using Moq;

[TestFixture]
public class HexMapStorageTests
{
    private Mock<IHexMapData> mockMapData;
    private HexMapStorage storage;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new HexGridData(1, 1, 1);
        mockMapData = new Mock<IHexMapData>();
        mockMapData.Setup(mock => mock.Deserialize()).Returns(new Dictionary<int, CubeHex>());
        storage = new HexMapStorage(mockMapData.Object);
    }

    [Test]
    public void Add_AddsHex_AtPosition()
    {
        var position = CubeHexVector.Zero;
        var hex = new CubeHex(position, new HexProperties(), new HexMeshData());
        var stored = storage.Get(position);
        Assert.That(stored, Is.Null);
        
        storage.Add(hex);
        
        stored = storage.Get(position);
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored, Is.EqualTo(hex));
    }

    [Test]
    public void Add_DoesNotThrow_WhenAddingSamePosition_Twice()
    {
        var hex = new CubeHex(0,0, new HexProperties(), new HexMeshData());
        storage.Add(hex);

        Assert.DoesNotThrow(() => storage.Add(hex));
    }

    [Test]
    public void Remove_DoesNotThrow_IfNothingAtPosition()
    {
        var position = CubeHexVector.Zero;

        Assert.DoesNotThrow(() => storage.Remove(position));
    }
    
    [Test]
    public void Remove_RemovesHex_AtPosition()
    {
        var position = CubeHexVector.Zero;
        var hex = new CubeHex(position, new HexProperties(), new HexMeshData());
        storage.Add(hex);
        var stored = storage.Get(position);
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored, Is.EqualTo(hex));
        
        storage.Remove(position);
        
        stored = storage.Get(position);
        Assert.That(stored, Is.Null);
    }

    [Test]
    public void Get_ReturnsNull_IfDoesNotExists()
    {
        var position = CubeHexVector.Zero;

        var stored = storage.Get(position);
        
        Assert.That(stored, Is.Null);
    }
    
    [Test]
    public void Get_ReturnsHex_IfExists()
    {
        var position = CubeHexVector.Zero;
        var hex = new CubeHex(position, new HexProperties(), new HexMeshData());
        storage.Add(hex);

        var stored = storage.Get(position);
        
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored, Is.EqualTo(hex));
    }

    [Test]
    public void GetMap_ReturnsNoHexes_IfEmpty()
    {
        var hexes = storage.GetMap();
        
        Assert.That(hexes, Is.Empty);
    }
    
    [Test]
    public void GetMap_ReturnsAllHexes_IfNotEmpty()
    {
        var hex = new CubeHex(0,0, new HexProperties(), new HexMeshData());
        storage.Add(hex);
        
        var hexes = storage.GetMap();
        
        Assert.That(hexes, Is.Not.Empty);
        Assert.That(hexes.Length, Is.EqualTo(1));
    }
}