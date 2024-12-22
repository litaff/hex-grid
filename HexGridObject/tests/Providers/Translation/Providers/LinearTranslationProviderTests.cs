namespace HexGridObject.Tests.Providers.Translation.Providers;

using global::HexGridObject.Providers.Translation;
using global::HexGridObject.Providers.Translation.Providers;
using HexGridMap;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class LinearTranslationProviderTests
{
    private LinearTranslationProvider provider;
    private Mock<ITranslatable> mockTranslatable;
    
    [SetUp]
    public void SetUp()
    {
        var gridData = new HexGridData(1, 1, 1);
        mockTranslatable = new Mock<ITranslatable>();
        provider = new LinearTranslationProvider(1, mockTranslatable.Object, new HeightData());
    }
    
    [Test]
    public void TranslateTo_DoesNotThrow_IfNoHexStateProvider()
    {
        Assert.DoesNotThrow(() => provider.TranslateTo(CubeHexVector.Zero));
    }
    
    [Test]
    public void TranslateTo_WorksProperly()
    {
        Assert.Inconclusive();
    }
    
    [Test]
    public void Update_WorksProperly()
    {
        Assert.Inconclusive();
    }
}