namespace HexGridObject.Tests.Handlers.Rotation;

using global::HexGridObject.Handlers.Rotation;
using HexGridMap;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class InstantRotationHandlerTestes
{
    private InstantRotationHandler handler;
    private Mock<IRotatable> mockRotatable;

    [SetUp]
    public void Setup()
    {
        mockRotatable = new Mock<IRotatable>();
        handler = new InstantRotationHandler(mockRotatable.Object);
    }

    [Test]
    public void RotateTowards_CallsLookTowardsOnIRotatable_WithWorldDirection()
    {
        var gridData = new HexGridProperties(1, 1, 1);
        
        handler.RotateTowards(CubeHexVector.North);
        
        mockRotatable.Verify(m => m.LookTowards(CubeHexVector.North.ToWorldPosition()), Times.Once);
    }
}