namespace HexGridObject.Tests;

using global::HexGridObject.Managers;
using global::HexGridObject.Providers;
using global::HexGridObject.Providers.Position;
using global::HexGridObject.Providers.Translation;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class HexGridObjectTests
{
    private Mock<IHexGridPositionProvider> mockPositionProvider;
    private Mock<ITranslationProvider> mockTranslationProvider;
    private Mock<IHexGridObjectLayerManager> mockLayerManager;
    private HexGridObject hexGridObject;
    private HeightData heightData;
    private HexStateProviders hexStateProviders;

    [SetUp]
    public void SetUp()
    {
        mockPositionProvider = new Mock<IHexGridPositionProvider>();
        mockTranslationProvider = new Mock<ITranslationProvider>();
        mockLayerManager = new Mock<IHexGridObjectLayerManager>();
        hexStateProviders = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            { 0, new Mock<IHexStateProvider>().Object }
        });
        heightData = new HeightData();
        hexGridObject = new HexGridObject(mockPositionProvider.Object, mockTranslationProvider.Object, heightData);
    }

    [Test]
    public void Enable_EnablesHexGridPositionProvider_WithHexStateProviders()
    {
        hexGridObject.Enable(mockLayerManager.Object, hexStateProviders);
        
        mockPositionProvider.Verify(p => p.Enable(hexStateProviders), Times.Once);
    }
    
    [Test]
    public void Enable_EnablesTranslationProvider_WithHexStateProvider_At0()
    {
        hexGridObject.Enable(mockLayerManager.Object, hexStateProviders);
        
        mockTranslationProvider.Verify(p => p.Enable(hexStateProviders.Providers[0]), Times.Once);
    }

    [Test]
    public void Disable_DisablesHexGridPositionProvider()
    {
        hexGridObject.Disable();

        mockPositionProvider.Verify(p => p.Disable(), Times.Once);
    }

    [Test]
    public void Disable_DisablesTranslationProvider()
    {
        hexGridObject.Disable();

        mockTranslationProvider.Verify(p => p.Disable(), Times.Once);
    }

    [Test]
    public void OnLayerChangeRequestedHandler_IsCalled_WithLayerChange()
    {
        hexGridObject.Enable(mockLayerManager.Object, hexStateProviders);
        hexGridObject.RegisterHandlers();

        const int relativeIndex = 1;
        mockPositionProvider.Raise(p => p.OnLayerChangeRequested += null, relativeIndex);

        mockLayerManager.Verify(m => m.ChangeGridObjectLayer(hexGridObject, relativeIndex), Times.Once);
    }

    [Test]
    public void OnHexGridPositionChangedHandler_IsCalled_WithPositionAndTranslationUpdates()
    {
        hexGridObject.Enable(mockLayerManager.Object, hexStateProviders);
        hexGridObject.RegisterHandlers();

        var oldPosition = new CubeHexVector(0, 0);
        var newPosition = new CubeHexVector(1, 1);
        mockPositionProvider.Setup(p => p.Position).Returns(newPosition);

        mockPositionProvider.Raise(p => p.OnPositionChanged += null, oldPosition);

        mockLayerManager.Verify(m => m.UpdateGridObjectPosition(hexGridObject, oldPosition), Times.Once);
        mockTranslationProvider.Verify(t => t.TranslateTo(newPosition), Times.Once);
    }
}