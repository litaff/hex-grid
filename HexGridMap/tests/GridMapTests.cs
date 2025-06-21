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
    private Mock<IRendererMapFactory> mockRendererMapFactory;
    private Mock<IRendererMap> mockRendererMap;
    private Mock<IHexMapData> mockHexMapData;
    
    [SetUp]
    public void SetUp()
    {
        mockHexMapData = new Mock<IHexMapData>();
        mockRendererMap = new Mock<IRendererMap>();
        mockMapDataProvider = new Mock<IHexMapDataProvider>();
        mockMapDataProvider.Setup(provider => provider.GetData()).Returns(new Dictionary<int, IHexMapData>());
        mockRendererMapFactory = new Mock<IRendererMapFactory>();
        gridMap = new GridMap(mockMapDataProvider.Object, mockRendererMapFactory.Object);
    }

    [Test]
    public void AddHex_CreatesLayer_WhenNoLayer()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
        mockRendererMapFactory.Setup(factory => factory.New(0)).Returns(mockRendererMap.Object);
        mockHexMapData.Setup(mock => mock.Map).Returns(new Dictionary<int, Map.Hex.Hex>());
        mockMapDataProvider.Setup(provider => provider.AddData(0)).Returns(mockHexMapData.Object);
        Assert.That(gridMap.Layers, Is.Empty);
        
        gridMap.AddHex(hex, 0);
        
        Assert.That(gridMap.Layers, Is.Not.Empty);
    }

    [Test]
    public void AddHex_AddsHexToExistingLayer()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
        mockRendererMapFactory.Setup(factory => factory.New(0)).Returns(mockRendererMap.Object);
        mockHexMapData.Setup(mock => mock.Map).Returns(new Dictionary<int, Map.Hex.Hex>());
        mockMapDataProvider.Setup(provider => provider.AddData(0)).Returns(mockHexMapData.Object);
        Assert.That(gridMap.Layers, Is.Empty);
        gridMap.AddHex(hex, 0);
        Assert.That(gridMap.Layers, Is.Not.Empty);
        Assert.That(gridMap.Layers.First().Value.GetHex(HexVector.Zero), Is.EqualTo(hex));
        hex = new Map.Hex.Hex(HexVector.Forward, new Properties(), new MeshData());

        gridMap.AddHex(hex, 0);
        
        Assert.That(gridMap.Layers.First().Value.GetHex(HexVector.Forward), Is.EqualTo(hex));
    }

    [Test]
    public void RemoveHex_RemovesHexFromLayer()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
        mockRendererMapFactory.Setup(factory => factory.New(0)).Returns(mockRendererMap.Object);
        mockHexMapData.Setup(mock => mock.Map).Returns(new Dictionary<int, Map.Hex.Hex>());
        mockMapDataProvider.Setup(provider => provider.AddData(0)).Returns(mockHexMapData.Object);
        Assert.That(gridMap.Layers, Is.Empty);
        gridMap.AddHex(hex, 0);
        Assert.That(gridMap.Layers, Is.Not.Empty);
        Assert.That(gridMap.Layers.First().Value.GetHex(HexVector.Zero), Is.EqualTo(hex));
        hex = new Map.Hex.Hex(HexVector.Forward, new Properties(), new MeshData());
        gridMap.AddHex(hex, 0);
        Assert.That(gridMap.Layers.First().Value.GetHex(HexVector.Forward), Is.EqualTo(hex));
        
        gridMap.RemoveHex(HexVector.Forward, 0);
        
        Assert.That(gridMap.Layers.First().Value.GetHex(HexVector.Forward), Is.Null);
    }
    
    [Test]
    public void RemoveHex_RemovesLayerWhenEmpty()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
        mockRendererMapFactory.Setup(factory => factory.New(0)).Returns(mockRendererMap.Object);
        mockHexMapData.Setup(mock => mock.Map).Returns(new Dictionary<int, Map.Hex.Hex>());
        mockMapDataProvider.Setup(provider => provider.AddData(0)).Returns(mockHexMapData.Object);
        Assert.That(gridMap.Layers, Is.Empty);
        gridMap.AddHex(hex, 0);
        Assert.That(gridMap.Layers, Is.Not.Empty);
        
        gridMap.RemoveHex(HexVector.Zero, 0);
        
        Assert.That(gridMap.Layers, Is.Empty);
    }
    
    [Test]
    public void GetHex_DoesNotThrow_WhenNoLayer()
    {
        Assert.DoesNotThrow(() => gridMap.GetHex(HexVector.Zero, 0));
    }

    [Test]
    public void GetHex_ReturnsNull_WhenIsLayerButNoHex()
    {
        var hex = new Map.Hex.Hex(HexVector.Zero, new Properties(), new MeshData());
        mockRendererMapFactory.Setup(factory => factory.New(0)).Returns(mockRendererMap.Object);
        mockHexMapData.Setup(mock => mock.Map).Returns(new Dictionary<int, Map.Hex.Hex>());
        mockMapDataProvider.Setup(provider => provider.AddData(0)).Returns(mockHexMapData.Object);
        Assert.That(gridMap.Layers, Is.Empty);
        gridMap.AddHex(hex, 0);
        Assert.That(gridMap.Layers, Is.Not.Empty);
        
        Assert.That(gridMap.GetHex(HexVector.Forward, 0), Is.Null);
    }
}