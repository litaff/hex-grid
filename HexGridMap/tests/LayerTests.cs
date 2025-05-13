namespace HexGrid.Map.Tests;

using Map.Hex;
using Map.Vector;
using Moq;
using Properties = Properties;

[TestFixture]
public class LayerTests
{
    private Mock<IHexMapData> mockHexData;
    private int layerIndex;
    private Layer layer;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new Properties(1, 1, 1);
        layerIndex = 0;
        mockHexData = new Mock<IHexMapData>();
        mockHexData.Setup(mock => mock.Deserialize()).Returns(new Dictionary<int, Map.Hex.Hex>());
        layer = new Layer(mockHexData.Object, layerIndex, null, null);
    }

    [Test]
    public void GetHex_ReturnsNull_IfNoHexAtPosition()
    {
        var position = HexVector.Zero;
        
        var hex = layer.GetHex(position);
        
        Assert.That(hex, Is.Null);
    }

    [Test]
    public void Works_As_Expected()
    {
        Assert.Inconclusive("Can't fully test. Most methods require MeshLibrary, which can't be created.");
    }
}