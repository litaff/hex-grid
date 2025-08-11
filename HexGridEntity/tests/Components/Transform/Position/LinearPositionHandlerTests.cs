namespace HexGrid.Entity.Tests.Components.Transform.Position;

using Godot;
using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Position.Interfaces;
using Map;
using Map.Vector;
using Moq;

[TestFixture]
public class LinearPositionHandlerTests
{
    private HexVectorConverter converter;
    private Properties properties;
    private LinearPositionHandler linearPositionHandler;
    private float speed;
    private Vector3 initialPosition;
    private Mock<IHeightManager> mockHeightManager;
    private Mock<IMovable> mockMovable;

    [SetUp]
    public void SetUp()
    {
        properties = new Properties(1, 1);
        converter = new HexVectorConverter(properties);
        initialPosition = Vector3.Zero;
        mockMovable = new Mock<IMovable>();
        mockMovable.Setup(mock => mock.Position).Returns(initialPosition);
        mockHeightManager = new Mock<IHeightManager>();
        speed = 1f;
        linearPositionHandler = new LinearPositionHandler(mockMovable.Object, mockHeightManager.Object, speed, converter);
    }

    [Test]
    public void MoveTo_CausesMoveCompleteToTurnFalseWhenPositionIsDifferentFromMovablePosition()
    {
        Assert.That(linearPositionHandler.MoveComplete, Is.True);
        linearPositionHandler.Enable();
        var position = HexVector.Forward;
        
        linearPositionHandler.MoveTo(position);
        
        Assert.That(linearPositionHandler.MoveComplete, Is.False);
    }

    [Test]
    public void MoveTo_DoesNotCallMovableOrInvokeOnPositionReadyWhenDisables()
    {
        linearPositionHandler.Disable();
        var readyCalled = false;
        linearPositionHandler.OnPositionReady += () => readyCalled = true;
        
        linearPositionHandler.MoveTo(HexVector.Zero);
        
        Assert.That(readyCalled, Is.False);
    }

    [Test]
    public void MoveTo_InvokesOnPositionReadyWhenMoveToIsValid()
    {
        mockMovable.Setup(movable => movable.Position).Returns(Vector3.Zero);
        mockHeightManager.Setup(manager => manager.GetHeight(HexVector.Zero, null)).Returns(0);
        linearPositionHandler.Enable();
        var readyCalled = false;
        linearPositionHandler.OnPositionReady += () => readyCalled = true;
        
        linearPositionHandler.MoveTo(HexVector.Zero);
        
        Assert.That(readyCalled, Is.True);
    }

    [Test]
    public void Enable_ChangesEnabledToTrue()
    {
        Assert.That(linearPositionHandler.Enabled, Is.False);
        
        linearPositionHandler.Enable();
        
        Assert.That(linearPositionHandler.Enabled, Is.True);
    }

    [Test]
    public void Disable_ChangesEnabledToFalse()
    {
        Assert.That(linearPositionHandler.Enabled, Is.False);
        linearPositionHandler.Enable();
        Assert.That(linearPositionHandler.Enabled, Is.True);
        
        linearPositionHandler.Disable();
        
        Assert.That(linearPositionHandler.Enabled, Is.False);
    }
    
    [Test]
    public void Update_TranslatesMovableByOneStepIfMovementIsMoreThanOneStepFromCompletion()
    {
        Assert.That(linearPositionHandler.MoveComplete, Is.True);
        linearPositionHandler.Enable();
        var position = HexVector.Forward;
        linearPositionHandler.MoveTo(position);
        Assert.That(linearPositionHandler.MoveComplete, Is.False);
        
        linearPositionHandler.Update(0);
        
        mockMovable.Verify(mock => mock.Translate(Vector3.Zero), Times.Once);
    }
    
    [Test]
    public void Update_TranslatesMovableByLeftoverOffsetWhenStepIsLargerThanLeftoverDistance()
    {
        Assert.That(linearPositionHandler.MoveComplete, Is.True);
        linearPositionHandler.Enable();
        var position = HexVector.Forward;
        linearPositionHandler.MoveTo(position);
        Assert.That(linearPositionHandler.MoveComplete, Is.False);
        
        linearPositionHandler.Update(100);
        
        mockMovable.Verify(mock => mock.Translate(converter.ToWorldPosition(position)), Times.Once);
    }
}