namespace HexGridObject.Tests.Handlers.Translation;

using global::HexGridObject.Handlers.Translation;
using Godot;
using HexGridMap;
using HexGridMap.Vector;
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
        var gridData = new HexGridProperties(1, 1, 1);
        initialPosition = Vector3.Zero;
        translationSpeed = 1f;
        mockTranslatable = new Mock<ITranslatable>();
        mockTranslatable.Setup(t => t.Position).Returns(initialPosition);
        handler = new LinearTranslationHandler(translationSpeed, mockTranslatable.Object, new HeightData());
    }
    
    [Test]
    public void TranslateTo_DoesNotThrow_IfNoHexStateProvider()
    {
        Assert.DoesNotThrow(() => handler.TranslateTo(CubeHexVector.Zero));
    }

    [Test]
    public void Update_DoesNotThrow_IfTranslationComplete_IsTrue()
    {
        Assert.DoesNotThrow(() => handler.Update(0d));
    }
}