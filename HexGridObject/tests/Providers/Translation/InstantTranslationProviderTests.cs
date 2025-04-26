namespace HexGridObject.Tests.Providers.Translation;

using global::HexGridObject.Providers;
using global::HexGridObject.Providers.Translation;
using Godot;
using HexGridMap;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class InstantTranslationProviderTests
{
    private InstantTranslationProvider provider;
    private Mock<ITranslatable> mockTranslatable;

    [SetUp]
    public void Setup()
    {
        mockTranslatable = new Mock<ITranslatable>();
        provider = new InstantTranslationProvider(mockTranslatable.Object, new HeightData());
    }

    [Test]
    public void TranslateTo_DoesNotThrow_IfNoHexStateProvider()
    {
        Assert.DoesNotThrow(() => provider.TranslateTo(CubeHexVector.Zero));
    }

    [Test]
    public void TranslateTo_CallsTranslate_FromITranslatable_WithCorrectOffset()
    {
        var gridData = new HexGridProperties(1, 1, 1);
        var mockProvider = new Mock<IHexStateProvider>();
        var position = CubeHexVector.Zero;
        mockProvider.Setup(m => m.GetHexHeight(position, null)).Returns(0);
        provider.Enable(mockProvider.Object);
        mockTranslatable.Setup(m => m.Position).Returns(Vector3.Zero);
        var offset = position.ToWorldPosition() - mockTranslatable.Object.Position;

        provider.TranslateTo(position);
        
        mockTranslatable.Verify(m => m.Translate(offset), Times.Once);
    }
}