namespace HexGridObject.Tests.Providers.Position;

using global::HexGridObject.Providers;
using global::HexGridObject.Providers.Position;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class HexGridPositionProviderTests
{
    private HexGridPositionProvider provider;
    private CubeHexVector initialPosition;
    private HeightData heightData;
    
    [SetUp]
    public void Setup()
    {
        initialPosition = CubeHexVector.Zero;
        heightData = new HeightData(1, 1);
        provider = new HexGridPositionProvider(initialPosition, heightData);
    }
    
    [Test]
    public void Translate_DoesNotThrow_IfNoProviders()
    {
        // Clear providers
        provider.Disable();

        Assert.DoesNotThrow(() => provider.Translate(CubeHexVector.Zero));
    }

    [Test]
    public void PlaneTranslate_ReturnsFalse_IfNoProviders()
    {        
        // Clear providers
        provider.Disable();
        
        Assert.That(provider.PlaneTranslate(CubeHexVector.Zero), Is.False);
    }
    
    [Test]
    public void PlaneTranslate_ReturnsFalse_WhenCanPlaneTransform_IsFalse()
    {
        var target = CubeHexVector.Zero;
        var mockStateProvider = new Mock<IHexStateProvider>();
        mockStateProvider.Setup(p => p.Exists(target)).Returns(false);
        var providers = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockStateProvider.Object}
        });
        provider.Enable(providers);
        
        Assert.That(provider.PlaneTranslate(target), Is.False);
        Assert.Warn("This is not super correct. CanPlaneTranslate should be injected.");
    }
    
    [Test]
    public void PlaneTranslate_ReturnsTrue_WhenCanPlaneTransform_IsTrue()
    {
        var target = CubeHexVector.Zero;
        var mockStateProvider = new Mock<IHexStateProvider>();
        mockStateProvider.Setup(p => p.Exists(target)).Returns(true);
        mockStateProvider.Setup(p => p.GetHexHeight(target)).Returns(0f);
        var providers = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockStateProvider.Object}
        });
        provider.Enable(providers);
        
        Assert.That(provider.PlaneTranslate(target), Is.True);
        Assert.Warn("This is not super correct. CanPlaneTranslate should be injected.");
    }
    
    [Test]
    public void PlaneTranslate_ChangesPositionToTarget()
    {
        var target = CubeHexVector.North;
        var mockStateProvider = new Mock<IHexStateProvider>();
        mockStateProvider.Setup(p => p.Exists(target)).Returns(true);
        mockStateProvider.Setup(p => p.GetHexHeight(target)).Returns(0f);
        var providers = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockStateProvider.Object}
        });
        provider.Enable(providers);
        Assert.That(provider.Position, Is.EqualTo(initialPosition));
        
        Assert.That(provider.PlaneTranslate(target), Is.True);
        
        Assert.That(provider.Position, Is.EqualTo(target));
    }
    
    [Test]
    public void PlaneTranslate_Invokes_OnPositionChanged()
    {
        var target = CubeHexVector.North;
        var mockStateProvider = new Mock<IHexStateProvider>();
        mockStateProvider.Setup(p => p.Exists(target)).Returns(true);
        mockStateProvider.Setup(p => p.GetHexHeight(target)).Returns(0f);
        var providers = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockStateProvider.Object}
        });
        provider.Enable(providers);
        var called = false;
        provider.OnPositionChanged += _ => called = true;
        
        Assert.That(provider.PlaneTranslate(target), Is.True);
        
        Assert.That(called, Is.True);
    }
    
    [Test]
    public void PlaneTranslate_Invokes_OnPositionChanged_WithPreviousPosition()
    {
        var target = CubeHexVector.North;
        var mockStateProvider = new Mock<IHexStateProvider>();
        mockStateProvider.Setup(p => p.Exists(target)).Returns(true);
        mockStateProvider.Setup(p => p.GetHexHeight(target)).Returns(0f);
        var providers = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockStateProvider.Object}
        });
        provider.Enable(providers);
        var previousPosition = target;
        provider.OnPositionChanged += position => previousPosition = position;
        
        Assert.That(provider.PlaneTranslate(target), Is.True);
        
        Assert.That(previousPosition, Is.EqualTo(initialPosition));
    }
    
    [Test]
    public void LayerTranslate_ReturnsFalse_IfNoProviders()
    {        
        // Clear providers
        provider.Disable();
        
        Assert.That(provider.LayerTranslate(CubeHexVector.Zero), Is.False);
    }

    // 
    [TestCase(1)]
    [TestCase(0)]
    [TestCase(-1)]
    public void LayerTranslateTo_ReturnsFalse_IfNoRelativeProvider(int relativeLayerIndex)
    {
        // Clear providers
        provider.Disable();
        
        Assert.That(provider.LayerTranslateTo(CubeHexVector.Zero, relativeLayerIndex), Is.False);
    }

    [Test]
    public void LayerTranslateTo_ReturnsFalse_IfNoHex_ForRelativeLayer()
    {
        var targetHexPosition = CubeHexVector.Zero;
        var mockHexStateProvider = new Mock<IHexStateProvider>();
        mockHexStateProvider.Setup(provider => provider.Exists(targetHexPosition)).Returns(false);
        
        provider.Enable(new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockHexStateProvider.Object}
        }));
        
        Assert.That(provider.LayerTranslateTo(CubeHexVector.Zero, 0), Is.False);
    }
}