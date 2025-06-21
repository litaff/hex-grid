namespace HexGrid.Entity.Tests.Handlers.Rotation;

using HexGrid.Entity.Handlers.Rotation;
using Map;
using Map.Vector;
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
        var gridData = new Properties(1, 1);
        
        handler.RotateTowards(HexVector.North);
        
        mockRotatable.Verify(m => m.LookTowards(HexVector.North.ToWorldPosition()), Times.Once);
    }
}