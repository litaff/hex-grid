namespace HexGrid.Entity.Tests.Components.Transform.Rotation;

using HexGrid.Entity.Components.Transform.Rotation;
using Map;
using Map.Vector;
using Moq;

[TestFixture]
public class RotationHandlerTests
{
    private Mock<IRotatable> mockRotatable;
    private RotationHandler rotationHandler;

    [SetUp]
    public void SetUp()
    {
        mockRotatable = new Mock<IRotatable>();
        rotationHandler = new RotationHandler(mockRotatable.Object);
    }

    [Test]
    public void RotateTowards_KeepsRotationCompleteTrue()
    {
        _ = new Properties(1, 1);   
        Assert.That(rotationHandler.RotationComplete, Is.True);
        
        rotationHandler.RotateTowards(HexVector.Forward);
        
        Assert.That(rotationHandler.RotationComplete, Is.True);
    }
    
    [Test]
    public void RotateTowards_CallsRotatableLookTowards()
    {
        _ = new Properties(1, 1);   
        
        rotationHandler.RotateTowards(HexVector.Forward);
        
        mockRotatable.Verify(mock => mock.LookTowards(HexVector.Forward.ToWorldPosition()), Times.Once);
    }

    [Test]
    public void RotateTowards_InvokesOnRotationHandled()
    {
        var called = false;
        rotationHandler.OnRotationHandled += () => called = true;
        _ = new Properties(1, 1);

        rotationHandler.RotateTowards(HexVector.Forward);
        
        Assert.That(called, Is.True);
    }
}