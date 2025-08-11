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
    private Properties properties;
    private HexVectorConverter converter;

    [SetUp]
    public void SetUp()
    {
        properties = new Properties(1, 1);
        converter = new HexVectorConverter(properties);
        mockMovable = new Mock<IMovable>();
        mockHeightManager = new Mock<IHeightManager>();
        instantPositionHandler = new InstantPositionHandler(mockMovable.Object, mockHeightManager.Object, converter);
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
        var previousPosition = Vector3.Zero;
        var newPosition = HexVector.Forward;
        var offset = converter.ToWorldPosition(newPosition) - previousPosition;
        mockMovable.Setup(movable => movable.Position).Returns(previousPosition);
        mockHeightManager.Setup(manager => manager.GetHeight(newPosition, null)).Returns(0);
        instantPositionHandler.Enable();
        
        instantPositionHandler.MoveTo(newPosition);
        
        mockMovable.Verify(movable => movable.Translate(offset), Times.Once);
    }

    [Test]
    public void MoveTo_InvokesOnPositionReadyWhenMoveToIsValid()
    {
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