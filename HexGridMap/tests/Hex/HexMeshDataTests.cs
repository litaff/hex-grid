namespace HexGridMap.Tests.Hex;

using global::HexGridMap.Hex;
using Godot;

[TestFixture]
public class HexMeshDataTests
{
    private HexMeshData hexMeshData;
    private int rotation;
    private int meshIndex;
    
    [SetUp]
    public void SetUp()
    {
        rotation = 0;
        meshIndex = 0;
        hexMeshData = new HexMeshData(meshIndex, rotation);
    }

    [Test]
    public void Radians_ReturnsRotation_InRadians()
    {
        Assert.That(hexMeshData.Radians, Is.EqualTo(Mathf.DegToRad(rotation)));
    }
}