namespace HexGrid.Entity.Tests.Handlers.Translation;

using Godot;
using HexGrid.Entity;
using HexGrid.Entity.Handlers.Translation;
using Map;
using Map.Vector;
using Moq;

[TestFixture]
public class LinearTranslationHandlerTests
{
    private LinearTranslationHandler handler;
    private Mock<ITranslatable> mockTranslatable;
    private Vector3 initialPosition;
    private float translationSpeed;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new Properties(1, 1, 1);
        initialPosition = Vector3.Zero;
        translationSpeed = 1f;
        mockTranslatable = new Mock<ITranslatable>();
        mockTranslatable.Setup(t => t.Position).Returns(initialPosition);
        handler = new LinearTranslationHandler(translationSpeed, mockTranslatable.Object, new HeightData());
    }
    
    [Test]
    public void TranslateTo_DoesNotThrow_IfNoHexStateProvider()
    {
        Assert.DoesNotThrow(() => handler.TranslateTo(HexVector.Zero));
    }

    [Test]
    public void Update_DoesNotThrow_IfTranslationComplete_IsTrue()
    {
        Assert.DoesNotThrow(() => handler.Update(0d));
    }
}