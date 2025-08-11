namespace HexGrid.Entity.Tests.Components.Transform.Position;

using Godot;
using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Position.Interfaces;
using Map;
using Map.Vector;
using Moq;

[TestFixture]
public class InstantPositionProviderTests
{
    private InstantPositionHandler instantPositionHandler;
    private Mock<IHeightManager> mockHeightManager;
    private Mock<IMovable> mockMovable;

    [SetUp]
    public void SetUp()
    {
        mockMovable = new Mock<IMovable>();
        mockHeightManager = new Mock<IHeightManager>();
        instantPositionHandler = new InstantPositionHandler(mockMovable.Object, mockHeightManager.Object);
    }

    [Test]
    public void MoveTo_DoesNotCallMovableOrInvokeOnPositionReadyWhenDisables()
    {
        mockMovable.Setup(movable => movable.Position).Returns(Vector3.Zero);
        mockHeightManager.Setup(manager => manager.GetHeight(HexVector.Zero, null)).Returns(0);
        instantPositionHandler.Disable();
        var readyCalled = false;
        instantPositionHandler.OnPositionReady += () => readyCalled = true;
        
        instantPositionHandler.MoveTo(HexVector.Zero);
        
        mockMovable.Verify(movable => movable.Translate(Vector3.Zero), Times.Never);
        Assert.That(readyCalled, Is.False);
    }

    [Test]
    public void MoveTo_LeavesMoveCompleteAtTrue()
    {
        _ = new Properties(1, 1);
        Assert.That(instantPositionHandler.MoveComplete, Is.True);
        mockMovable.Setup(movable => movable.Position).Returns(Vector3.Zero);
        mockHeightManager.Setup(manager => manager.GetHeight(HexVector.Zero, null)).Returns(0);
        instantPositionHandler.Enable();
        
        instantPositionHandler.MoveTo(HexVector.Zero);
        
        Assert.That(instantPositionHandler.MoveComplete, Is.True);
    }

    [Test]
    public void MoveTo_MovesMovableByOffsetFromGivenPositionAndMovablePosition()
    {
        _ = new Properties(1, 1);
        var previousPosition = Vector3.Zero;
        var newPosition = HexVector.Forward;
        var offset = newPosition.ToWorldPosition() - previousPosition;
        mockMovable.Setup(movable => movable.Position).Returns(previousPosition);
        mockHeightManager.Setup(manager => manager.GetHeight(newPosition, null)).Returns(0);
        instantPositionHandler.Enable();
        
        instantPositionHandler.MoveTo(newPosition);
        
        mockMovable.Verify(movable => movable.Translate(offset), Times.Once);
    }

    [Test]
    public void MoveTo_InvokesOnPositionReadyWhenMoveToIsValid()
    {
        _ = new Properties(1, 1);
        mockMovable.Setup(movable => movable.Position).Returns(Vector3.Zero);
        mockHeightManager.Setup(manager => manager.GetHeight(HexVector.Zero, null)).Returns(0);
        instantPositionHandler.Enable();
        var readyCalled = false;
        instantPositionHandler.OnPositionReady += () => readyCalled = true;
        
        instantPositionHandler.MoveTo(HexVector.Zero);
        
        Assert.That(readyCalled, Is.True);
    }

    [Test]
    public void Enable_ChangesEnabledToTrue()
    {
        Assert.That(instantPositionHandler.Enabled, Is.False);
        
        instantPositionHandler.Enable();
        
        Assert.That(instantPositionHandler.Enabled, Is.True);
    }

    [Test]
    public void Disable_ChangesEnabledToFalse()
    {
        Assert.That(instantPositionHandler.Enabled, Is.False);
        instantPositionHandler.Enable();
        Assert.That(instantPositionHandler.Enabled, Is.True);
        
        instantPositionHandler.Disable();
        
        Assert.That(instantPositionHandler.Enabled, Is.False);
    }
}