namespace HexGridMap.Tests.Layer;

using global::HexGridMap.Hex;
using global::HexGridMap.Layer;
using global::HexGridMap.Storage;
using global::HexGridMap.Vector;
using Moq;

[TestFixture]
public class LayerStorageTests
{
    private Mock<IHexMapData> mockHexMapData;
    private int layerIndex;
    private LayerStorage layerStorage;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new HexGridData(1, 1, 1);
        layerIndex = 0;
        mockHexMapData = new Mock<IHexMapData>();
        mockHexMapData.Setup(mock => mock.Deserialize()).Returns(new Dictionary<int, CubeHex>());
        layerStorage = new LayerStorage(mockHexMapData.Object, layerIndex, null, null);
    }

    [Test]
    public void GetHex_ReturnsNull_IfNoHexAtPosition()
    {
        var position = CubeHexVector.Zero;
        
        var hex = layerStorage.GetHex(position);
        
        Assert.That(hex, Is.Null);
    }

    [Test]
    public void Works_As_Expected()
    {
        Assert.Inconclusive("Can't fully test. Most methods require MeshLibrary, which can't be created.");
    }
}