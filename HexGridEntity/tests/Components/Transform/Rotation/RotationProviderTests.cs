namespace HexGrid.Entity.Tests.Components.Transform.Rotation;

using HexGrid.Entity.Components.Transform.Rotation;
using Map.Vector;
using Moq;

[TestFixture]
public class RotationProviderTests
{
    private RotationProvider rotationProvider;
    private Mock<IRotationHandler> mockRotationHandler;
    private HexVector initialForward;

    [SetUp]
    public void SetUp()
    {
        initialForward = HexVector.Forward;
        mockRotationHandler = new Mock<IRotationHandler>();
        rotationProvider = new RotationProvider(initialForward);
    }

    [Test]
    public void Enable_TurnsEnabledTrue()
    {
        Assert.That(rotationProvider.Enabled, Is.False);
        
        rotationProvider.Enable();
        
        Assert.That(rotationProvider.Enabled, Is.True);
    }

    [Test]
    public void Enable_InvokesOnRotationChanged()
    {
        var called = false;
        rotationProvider.OnRotationChanged += () => called = true;
        
        rotationProvider.Enable();
        
        Assert.That(called, Is.True);
    }

    [Test]
    public void Disable_TurnsEnabledFalse()
    {
        Assert.That(rotationProvider.Enabled, Is.False);
        rotationProvider.Enable();
        Assert.That(rotationProvider.Enabled, Is.True);
        
        rotationProvider.Disable();
        
        Assert.That(rotationProvider.Enabled, Is.False);
    }

    [Test]
    public void Updated_DoesNotCallUpdateOnExistingHandlerWhenDisabled()
    {
        rotationProvider.Disable();
        rotationProvider.SetHandler(mockRotationHandler.Object);
        
        rotationProvider.Update(0);
        
        mockRotationHandler.Verify(mock => mock.Update(0), Times.Never);
    }

    [Test]
    public void Update_CallsUpdateOnHandlerWhenEnabled()
    {
        rotationProvider.Enable();
        rotationProvider.SetHandler(mockRotationHandler.Object);
        
        rotationProvider.Update(0);
        
        mockRotationHandler.Verify(mock => mock.Update(0), Times.Once);
    }
    
    [Test]
    public void RotateTowards_DoesNotChangeForwardRotationWhenDisabledAndTargetIsDifferentThanCurrentForward()
    {
        var target = HexVector.Backward;
        var forward = rotationProvider.Forward;
        rotationProvider.Disable();
        
        rotationProvider.RotateTowards(target);
        
        Assert.That(rotationProvider.Forward, Is.EqualTo(forward));
    }

    [Test]
    public void RotateTowards_ChangesForwardRotationWhenEnabledAndTargetIsDifferentThanCurrentForward()
    {
        var target = HexVector.Backward;
        rotationProvider.Enable();
        
        rotationProvider.RotateTowards(target);
        
        Assert.That(rotationProvider.Forward, Is.EqualTo(target));
    }

    [Test]
    public void RotateTowards_DoesNotChangeForwardRotationWhenEnabledAndTargetIsTheSameAsForward()
    {
        var target = rotationProvider.Forward;
        var forward = rotationProvider.Forward;
        rotationProvider.Enable();
        
        rotationProvider.RotateTowards(target);
        
        Assert.That(rotationProvider.Forward, Is.EqualTo(forward));
    }

    [Test]
    public void RotateTowards_ChangesForwardImmediatelyWhenNoHandler()
    {
        var target = rotationProvider.Backward;
        rotationProvider.Enable();
        
        rotationProvider.RotateTowards(target);
        
        Assert.That(rotationProvider.Forward, Is.EqualTo(target));
    }

    [Test]
    public void RotateTowards_InvokesOnRotationChangedImmediatelyWhenNoHandler()
    {
        var target = rotationProvider.Backward;
        var called = false;
        rotationProvider.OnRotationChanged += () => called = true;
        rotationProvider.Enable();
        
        rotationProvider.RotateTowards(target);
        
        Assert.That(called, Is.True);
    }

    [Test]
    public void RotateTowards_AssignsToOnRotationHandledWhenHandlerSet()
    {
        var target = rotationProvider.Backward;
        rotationProvider.SetHandler(mockRotationHandler.Object);
        rotationProvider.Enable();
        
        rotationProvider.RotateTowards(target);

        mockRotationHandler.VerifyAdd(mock => mock.OnRotationHandled += It.IsAny<Action>(), Times.Once);
    }

    [Test]
    public void RotateTowards_CallsRotateTowardsOnHandlerIfSet()
    {
        var target = rotationProvider.Backward;
        rotationProvider.SetHandler(mockRotationHandler.Object);
        rotationProvider.Enable();
        
        rotationProvider.RotateTowards(target);

        mockRotationHandler.Verify(mock => mock.RotateTowards(target), Times.Once);
    }
}