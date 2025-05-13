namespace HexGrid.Map.Tests.Hex;

using Godot;
using Map.Hex;

[TestFixture]
public class MeshDataTests
{
    private MeshData meshData;
    private int rotation;
    private int meshIndex;
    
    [SetUp]
    public void SetUp()
    {
        rotation = 0;
        meshIndex = 0;
        meshData = new MeshData(meshIndex, rotation);
    }

    [Test]
    public void Radians_ReturnsRotation_InRadians()
    {
        Assert.That(meshData.Radians, Is.EqualTo(Mathf.DegToRad(rotation)));
    }
}