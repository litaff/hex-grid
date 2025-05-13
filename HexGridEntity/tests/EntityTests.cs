namespace HexGrid.Entity.Tests;

using HexGrid.Entity;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Handlers.Translation;
using HexGrid.Entity.Managers;
using HexGrid.Entity.Providers;
using HexGrid.Entity.Providers.Block;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using Map.Vector;
using Moq;

[TestFixture]
public class EntityTests
{
    private Mock<IPositionProvider> mockPositionProvider;
    private Mock<IRotationProvider> mockRotationProvider;
    private Mock<ITranslationHandler> mockTranslationHandler;
    private Mock<IRotationHandler> mockRotationHandler;
    private Mock<IBlockProvider> mockBlockProvider;
    private Mock<IEntityLayerManager> mockLayerManager;
    private Entity entity;
    private HeightData heightData;
    private HexStateProviders hexStateProviders;

    [SetUp]
    public void SetUp()
    {
        mockPositionProvider = new Mock<IPositionProvider>();
        mockRotationProvider = new Mock<IRotationProvider>();
        mockTranslationHandler = new Mock<ITranslationHandler>();
        mockRotationHandler = new Mock<IRotationHandler>();
        mockBlockProvider = new Mock<IBlockProvider>();
        mockLayerManager = new Mock<IEntityLayerManager>();
        hexStateProviders = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            { 0, new Mock<IHexStateProvider>().Object }
        });
        heightData = new HeightData();
        entity = new Entity(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, mockBlockProvider.Object, heightData);
    }

    [Test]
    public void Enable_EnablesHexGridPositionProvider_WithHexStateProviders()
    {
        entity.Enable(mockLayerManager.Object, hexStateProviders);
        
        mockPositionProvider.Verify(p => p.Enable(hexStateProviders), Times.Once);
    }
    
    [Test]
    public void Enable_EnablesTranslationProvider_WithHexStateProvider_At0()
    {
        entity.Enable(mockLayerManager.Object, hexStateProviders);
        
        mockTranslationHandler.Verify(p => p.Enable(hexStateProviders.Providers[0]), Times.Once);
    }

    [Test]
    public void Disable_DisablesHexGridPositionProvider()
    {
        entity.Disable();

        mockPositionProvider.Verify(p => p.Disable(), Times.Once);
    }

    [Test]
    public void Disable_DisablesTranslationProvider()
    {
        entity.Disable();

        mockTranslationHandler.Verify(p => p.Disable(), Times.Once);
    }

    [Test]
    public void OnLayerChangeRequestedHandler_IsCalled_WithLayerChange()
    {
        entity.Enable(mockLayerManager.Object, hexStateProviders);
        entity.RegisterHandlers();

        const int relativeIndex = 1;
        mockPositionProvider.Raise(p => p.OnLayerChangeRequested += null, relativeIndex);

        mockLayerManager.Verify(m => m.ChangeLayer(entity, relativeIndex), Times.Once);
    }

    [Test]
    public void OnPositionChangedHandler_IsCalled_WithPositionAndTranslationUpdates()
    {
        entity.Enable(mockLayerManager.Object, hexStateProviders);
        entity.RegisterHandlers();

        var oldPosition = new HexVector(0, 0);
        var newPosition = new HexVector(1, 1);
        mockPositionProvider.Setup(p => p.Position).Returns(newPosition);

        mockPositionProvider.Raise(p => p.OnPositionChanged += null, oldPosition);

        mockLayerManager.Verify(m => m.UpdatePosition(entity, oldPosition), Times.Once);
        mockTranslationHandler.Verify(t => t.TranslateTo(newPosition), Times.Once);
    }
}