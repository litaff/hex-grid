namespace HexGrid.Entity.Tests.Handlers.Translation;

using Godot;
using HexGrid.Entity;
using HexGrid.Entity.Handlers.Position;
using HexGrid.Entity.Providers;
using Map;
using Map.Vector;
using Moq;

[TestFixture]
public class InstantPositionHandlerTests
{
    private InstantPositionHandler handler;
    private Mock<ITranslatable> mockTranslatable;

    [SetUp]
    public void Setup()
    {
        mockTranslatable = new Mock<ITranslatable>();
        handler = new InstantPositionHandler(mockTranslatable.Object, new HeightData());
    }

    [Test]
    public void TranslateTo_DoesNotThrow_IfNoHexStateProvider()
    {
        Assert.DoesNotThrow(() => handler.TranslateTo(HexVector.Zero));
    }

    [Test]
    public void TranslateTo_CallsTranslate_FromITranslatable_WithCorrectOffset()
    {
        var gridData = new Properties(1, 1);
        var mockProvider = new Mock<IHexStateProvider>();
        var position = HexVector.Zero;
        mockProvider.Setup(m => m.GetHexHeight(position, null)).Returns(0);
        handler.Enable(mockProvider.Object);
        mockTranslatable.Setup(m => m.Position).Returns(Vector3.Zero);
        var offset = position.ToWorldPosition() - mockTranslatable.Object.Position;

        handler.TranslateTo(position);
        
        mockTranslatable.Verify(m => m.Translate(offset), Times.Once);
    }
}