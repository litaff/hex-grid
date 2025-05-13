namespace HexGrid.Entity.Tests.Managers;

using HexGrid.Entity;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Handlers.Translation;
using HexGrid.Entity.Managers;
using HexGrid.Entity.Providers.Block;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using Map.Hex;
using Map.Vector;
using Moq;

[TestFixture]
public class EntityManagerTests
{
    private EntityManager manager;

    [SetUp]
    public void Setup()
    {
        var hexProviders = new Dictionary<int, IHexProvider>
        {
            { 0, new Mock<IHexProvider>().Object }
        };
        manager = new EntityManager(hexProviders);
    }

    [Test]
    public void Add_Returns_IfNoLayerManagerAtLayer()
    {
        var entity = GetMockEntity();
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
        
        manager.Add(entity, 1);
        
        // Checks if layer manager wasn't added, but should be ok.
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
    }

    [Test]
    public void Add_CallsAdd_FromLayerManager()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void Add_CallsEnable_FromEntity()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void Add_CallsRegisterHandlers_FromEntity()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void Remove_Returns_IfNoLayerManagerAtLayer()
    {
        var entity = GetMockEntity();
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
        
        manager.Remove(entity, 1);
        
        // Checks if layer manager wasn't added, but should be ok.
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
    }

    [Test]
    public void Remove_CallsRemove_FromLayerManager()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void RemoveEntity_CallsDisable_FromEntity()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void RemoveEntity_CallsUnregisterHandlers_FromEntity()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    private Entity GetMockEntity(
        Mock<IPositionProvider>? mockPositionProvider = null,
        Mock<IRotationProvider>? mockRotationProvider = null,
        Mock<ITranslationHandler>? mockTranslationHandler = null,
        Mock<IRotationHandler>? mockRotationHandler = null,
        Mock<IBlockProvider>? mockBlockProvider = null,
        HeightData heightData = new())
    {
        mockPositionProvider ??= new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(HexVector.Zero);
        mockRotationProvider ??= new Mock<IRotationProvider>();
        mockTranslationHandler ??= new Mock<ITranslationHandler>();
        mockRotationHandler ??= new Mock<IRotationHandler>();
        mockBlockProvider ??= new Mock<IBlockProvider>();
        return new Entity(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, mockBlockProvider.Object, heightData);
    }
}