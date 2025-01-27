namespace HexGridMap.Tests.Hex;

using global::HexGridMap.Hex;
using global::HexGridMap.Vector;

[TestFixture]
public class CubeHexTests
{
    [Test]
    public void IntCoordinateConstructor_SetsPositionCorrectly()
    {
        var hex = new CubeHex(0, 0, new HexProperties(), new HexMeshData());
        
        Assert.That(hex.Position, Is.EqualTo(CubeHexVector.Zero));
    }
}