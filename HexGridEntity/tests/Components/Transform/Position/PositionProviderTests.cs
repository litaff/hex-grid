namespace HexGrid.Entity.Tests.Components.Transform.Position;

using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Position.Interfaces;
using HexGrid.Entity.Managers;
using Map.Vector;
using Moq;

[TestFixture]
public class PositionProviderTests
{
    private Mock<IPositionHandler> mockPositionHandler;
    private Mock<IHexManager> mockHexManager;
    private Mock<IBlockerManager> mockBlockManager;
    private HexVector initialPosition;
    private int step;
    private PositionProvider positionProvider;

    [SetUp]
    public void SetUp()
    {
        mockHexManager = new Mock<IHexManager>();
        mockBlockManager = new Mock<IBlockerManager>();
        mockPositionHandler = new Mock<IPositionHandler>();
        initialPosition = HexVector.Zero;
        step = 1;
        positionProvider = new PositionProvider(mockHexManager.Object, mockBlockManager.Object, initialPosition, step);
    }

    [Test]
    public void Enable_EnablesHandler()
    {
        positionProvider.SetHandler(mockPositionHandler.Object);
        
        positionProvider.Enable();
        
        mockPositionHandler.Verify(mock => mock.Enable(), Times.Once);
    }

    [Test]
    public void Enable_TurnsEnabledTrue()
    {
        Assert.That(positionProvider.Enabled, Is.False);
        
        positionProvider.Enable();
        
        Assert.That(positionProvider.Enabled, Is.True);
    }

    [Test]
    public void Enable_InvokesOnPositionChanged()
    {
        var called = false;
        positionProvider.OnPositionChanged += () => called = true;
        
        positionProvider.Enable();
        
        Assert.That(called, Is.True);
    }

    [Test]
    public void Disable_DisablesHandler()
    {
        positionProvider.SetHandler(mockPositionHandler.Object);
        
        positionProvider.Disable();
        
        mockPositionHandler.Verify(mock => mock.Disable(), Times.Once);
    }

    [Test]
    public void Disable_TurnsEnabledFalse()
    {
        Assert.That(positionProvider.Enabled, Is.False);
        positionProvider.Enable();
        Assert.That(positionProvider.Enabled, Is.True);
        
        positionProvider.Disable();
        
        Assert.That(positionProvider.Enabled, Is.False);
    }

    [Test]
    public void Update_DoesNotCallHandlersUpdateIfDisabled()
    {
        positionProvider.SetHandler(mockPositionHandler.Object);
        
        positionProvider.Update(0);
        
        mockPositionHandler.Verify(mock => mock.Update(0), Times.Never);
    }

    [Test]
    public void Update_CallsHandlesUpdateWhenEnabled()
    {
        positionProvider.SetHandler(mockPositionHandler.Object);
        positionProvider.Enable();
        
        positionProvider.Update(0);
        
        mockPositionHandler.Verify(mock => mock.Update(0), Times.Once);
    }

    [Test]
    public void CanTranslate_ReturnsFalseWhenNoHexFound()
    {
        var offset = HexVector.Forward;
        var target = initialPosition + offset;
        var closest = HexVector.Zero;
        positionProvider.Enable();
        mockHexManager.Setup(mock => mock.TryGetClosest(target, step, out closest)).Returns(false);

        var can = positionProvider.CanTranslate(offset, out closest);
        
        Assert.That(can, Is.False);
    }

    [Test]
    public void CanTranslate_OutputsCurrentPositionWhenNoClosest()
    {
        var offset = HexVector.Forward;
        var target = initialPosition + offset;
        var closest = HexVector.Zero;
        positionProvider.Enable();
        mockHexManager.Setup(mock => mock.TryGetClosest(target, step, out closest)).Returns(false);

        positionProvider.CanTranslate(offset, out var newClosest);
        
        Assert.That(newClosest, Is.EqualTo(initialPosition));
    }

    [Test]
    public void CanTranslate_CallsAllValidatorsWhenClosestFound()
    {
        Assert.Inconclusive("Validators not implemented yet...");
    }

    [Test]
    public void Translate_UpdatesPositionImmediatelyWhenNoHandler()
    {
        Assert.That(positionProvider.Current, Is.EqualTo(initialPosition));
        Assert.That(positionProvider.Previous, Is.EqualTo(initialPosition));
        var offset = HexVector.Forward;
        var target = initialPosition + offset;
        var closest = HexVector.Forward;
        positionProvider.Enable();
        mockBlockManager.Setup(mock => mock.IsBlocked(target, initialPosition.DirectionTo(target))).Returns(false);
        mockBlockManager.Setup(mock => mock.IsBlocked(initialPosition, target.DirectionTo(initialPosition))).Returns(false);
        mockHexManager.Setup(mock => mock.TryGetClosest(target, step, out closest)).Returns(true);

        positionProvider.Translate(offset);
        
        Assert.That(positionProvider.Current, Is.EqualTo(target));
        Assert.That(positionProvider.Previous, Is.EqualTo(initialPosition));
    }
    
    [Test]
    public void Translate_InvokesOnPositionChangedImmediatelyWhenNoHandler()
    {
        var offset = HexVector.Forward;
        var target = initialPosition + offset;
        var closest = HexVector.Forward;
        var called = false;
        positionProvider.OnPositionChanged += () => called = true;
        positionProvider.Enable();
        mockBlockManager.Setup(mock => mock.IsBlocked(target, initialPosition.DirectionTo(target))).Returns(false);
        mockBlockManager.Setup(mock => mock.IsBlocked(initialPosition, target.DirectionTo(initialPosition))).Returns(false);
        mockHexManager.Setup(mock => mock.TryGetClosest(target, step, out closest)).Returns(true);

        positionProvider.Translate(offset);
        
        Assert.That(called, Is.True);
    }

    [Test]
    public void Translate_AssignsToPositionReadyOfHandlerWhenItIsSet()
    {
        var offset = HexVector.Forward;
        var target = initialPosition + offset;
        var closest = HexVector.Forward;
        positionProvider.SetHandler(mockPositionHandler.Object);
        positionProvider.Enable();
        mockBlockManager.Setup(mock => mock.IsBlocked(target, initialPosition.DirectionTo(target))).Returns(false);
        mockBlockManager.Setup(mock => mock.IsBlocked(initialPosition, target.DirectionTo(initialPosition))).Returns(false);
        mockHexManager.Setup(mock => mock.TryGetClosest(target, step, out closest)).Returns(true);

        positionProvider.Translate(offset);
        
        mockPositionHandler.VerifyAdd(mock => mock.OnPositionReady += It.IsAny<Action>(), Times.Once);
    }
    
    [Test]
    public void Translate_CallsMoveToFromHandlerWhenItIsSet()
    {
        var offset = HexVector.Forward;
        var target = initialPosition + offset;
        var closest = HexVector.Forward;
        positionProvider.SetHandler(mockPositionHandler.Object);
        positionProvider.Enable();
        mockBlockManager.Setup(mock => mock.IsBlocked(target, initialPosition.DirectionTo(target))).Returns(false);
        mockBlockManager.Setup(mock => mock.IsBlocked(initialPosition, target.DirectionTo(initialPosition))).Returns(false);
        mockHexManager.Setup(mock => mock.TryGetClosest(target, step, out closest)).Returns(true);

        positionProvider.Translate(offset);
        
        mockPositionHandler.Verify(mock => mock.MoveTo(target), Times.Once);
    }
}