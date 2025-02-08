namespace HexGridMap.Tests;

using global::HexGridMap.Hex;
using global::HexGridMap.Vector;
using Moq;

[TestFixture]
public class HexGridLayerTests
{
    private Mock<IHexData> mockHexData;
    private int layerIndex;
    private HexGridLayer hexGridLayer;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new HexGridProperties(1, 1, 1);
        layerIndex = 0;
        mockHexData = new Mock<IHexData>();
        mockHexData.Setup(mock => mock.Deserialize()).Returns(new Dictionary<int, CubeHex>());
        hexGridLayer = new HexGridLayer(mockHexData.Object, layerIndex, null, null);
    }

    [Test]
    public void GetHex_ReturnsNull_IfNoHexAtPosition()
    {
        var position = CubeHexVector.Zero;
        
        var hex = hexGridLayer.GetHex(position);
        
        Assert.That(hex, Is.Null);
    }

    [Test]
    public void Works_As_Expected()
    {
        Assert.Inconclusive("Can't fully test. Most methods require MeshLibrary, which can't be created.");
    }
}