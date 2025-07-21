namespace HexGrid.Map.Tests;

using Map.Hex;
using Map.Renderer;
using Map.Vector;
using Moq;

[TestFixture]
public class GridMapTests
{
    private GridMap gridMap;
    private Mock<IHexMapDataProvider> mockMapDataProvider;
    private Mock<IRendererMap> mockRendererMap;
    private Mock<IHexMapData> mockHexMapData;
    
    [SetUp]
    public void SetUp()
    {
        mockHexMapData = new Mock<IHexMapData>();
        mockHexMapData.Setup(data => data.Map).Returns(new Dictionary<int, Map.Hex.Hex>());
        mockRendererMap = new Mock<IRendererMap>();
        mockMapDataProvider = new Mock<IHexMapDataProvider>();
        mockMapDataProvider.Setup(provider => provider.GetData()).Returns(mockHexMapData.Object);
        gridMap = new GridMap(mockMapDataProvider.Object, mockRendererMap.Object);
    }

    [Test]
    public void AddHex_AddsHex_ToHexMap()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
    
        gridMap.AddHex(hex);
        
        Assert.That(gridMap.HexMap.Get(hex.Position), Is.Not.Null);
    }

    [Test]
    public void AddHex_AddsHex_ToRendererMap()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
    
        gridMap.AddHex(hex);
        
        mockRendererMap.Verify(map => map.AddHex(hex), Times.Once);
    }
    
    [Test]
    public void RemoveHex_RemovesHex_FromHexMap()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
        gridMap.AddHex(hex);
        Assert.That(gridMap.HexMap.Get(hex.Position), Is.Not.Null);
        
        gridMap.RemoveHex(hex.Position);
        
        Assert.That(gridMap.HexMap.Get(hex.Position), Is.Null);
    }

    [Test]
    public void RemoveHex_RemovesHex_FromRendererMap()
    {
        var position = HexVector.Zero;
        gridMap.RemoveHex(position);
        
        mockRendererMap.Verify(map => map.RemoveHex(position), Times.Once);
    }
    
    [Test]
    public void GetHex_DoesNotThrow_WhenNoHex()
    {
        Assert.DoesNotThrow(() => gridMap.GetHex(HexVector.Zero));
    }

    [Test]
    public void GetHex_ReturnsNull_WhenNoHex()
    {
        Assert.That(gridMap.GetHex(HexVector.Zero), Is.Null);
    }

    [Test]
    public void Dispose_Disposes_RendererMap()
    {
        gridMap.Dispose();
        
        mockRendererMap.Verify(map => map.Dispose(), Times.Once);
    }

    [Test]
    public void Occlude_Works()
    {
        Assert.Fail();
    }
}