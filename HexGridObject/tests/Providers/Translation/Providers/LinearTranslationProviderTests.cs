namespace HexGridObject.Tests.Providers.Translation.Providers;

using global::HexGridObject.Providers;
using global::HexGridObject.Providers.Translation;
using global::HexGridObject.Providers.Translation.Providers;
using Godot;
using HexGridMap;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class LinearTranslationProviderTests
{
    private LinearTranslationProvider provider;
    private Mock<ITranslatable> mockTranslatable;
    private Vector3 initialPosition;
    private float translationSpeed;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new HexGridData(1, 1, 1);
        initialPosition = Vector3.Zero;
        translationSpeed = 1f;
        mockTranslatable = new Mock<ITranslatable>();
        mockTranslatable.Setup(t => t.Position).Returns(initialPosition);
        provider = new LinearTranslationProvider(translationSpeed, mockTranslatable.Object, new HeightData());
    }
    
    [Test]
    public void TranslateTo_DoesNotThrow_IfNoHexStateProvider()
    {
        Assert.DoesNotThrow(() => provider.TranslateTo(CubeHexVector.Zero));
    }

    [Test]
    public void Update_DoesNotThrow_IfTranslationComplete_IsTrue()
    {
        Assert.DoesNotThrow(() => provider.Update(0d));
    }
}