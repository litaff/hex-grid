namespace HexGrid.Entity.Tests.Components.Transform.Rotation;

using HexGrid.Entity.Components.Transform.Rotation;
using Map;
using Map.Vector;
using Moq;

[TestFixture]
public class RotationHandlerTests
{
    private Properties properties;
    private HexVectorConverter converter;
    private Mock<IRotatable> mockRotatable;
    private RotationHandler rotationHandler;

    [SetUp]
    public void SetUp()
    {
        properties = new Properties(1, 1);
        converter = new HexVectorConverter(properties);
        mockRotatable = new Mock<IRotatable>();
        rotationHandler = new RotationHandler(mockRotatable.Object, converter);
    }

    [Test]
    public void RotateTowards_KeepsRotationCompleteTrue()
    {
        Assert.That(rotationHandler.RotationComplete, Is.True);
        
        rotationHandler.RotateTowards(HexVector.Forward);
        
        Assert.That(rotationHandler.RotationComplete, Is.True);
    }
    
    [Test]
    public void RotateTowards_CallsRotatableLookTowards()
    {
        rotationHandler.RotateTowards(HexVector.Forward);
        
        mockRotatable.Verify(mock => mock.LookTowards(converter.ToWorldPosition(HexVector.Forward)), Times.Once);
    }

    [Test]
    public void RotateTowards_InvokesOnRotationHandled()
    {
        var called = false;
        rotationHandler.OnRotationHandled += () => called = true;

        rotationHandler.RotateTowards(HexVector.Forward);
        
        Assert.That(called, Is.True);
    }
}