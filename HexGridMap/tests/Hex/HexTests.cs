namespace HexGrid.Map.Tests.Hex;

using Map.Hex;
using Map.Vector;

[TestFixture]
public class HexTests
{
    [Test]
    public void IntCoordinateConstructor_SetsPositionCorrectly()
    {
        var hex = new Hex(0, 0, new Properties(), new MeshData());
        
        Assert.That(hex.Position, Is.EqualTo(HexVector.Zero));
    }
}