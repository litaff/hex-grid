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
using Properties = Map.Hex.Properties;

[TestFixture]
public class EntityLayerManagerTests
{
    private EntityLayerManager layerManager;
    private Mock<IHexProvider> mockHexProvider;
    private Mock<IEntityManager> mockEntityManager;

    [SetUp]
    public void Setup()
    {
        mockHexProvider = new Mock<IHexProvider>();
        mockEntityManager = new Mock<IEntityManager>();
        layerManager = new EntityLayerManager(0, 
            mockHexProvider.Object, 
            mockEntityManager.Object);
    }

    [Test]
    public void Add_AddsANewStackWithEntity_IfThereWasNoStackAtPosition()
    {
        var entity = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);

        layerManager.Add(entity);
        
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
    }
    
    [Test]
    public void Add_AddsEntityToExistingStack_IfThereWasStackAtPosition()
    {
        var entity = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.Add(entity);
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
        entity = GetMockEntity();
        
        layerManager.Add(entity);
        
        Assert.That(stack.Entities, Does.Contain(entity));
    }

    [Test]
    public void Remove_Returns_IfNoStackAtPosition()
    {
        var hexGridObject = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);

        layerManager.Remove(hexGridObject);
        
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
    }

    [Test]
    public void Remove_RemovesEntity_IfStackExistsAtPosition()
    {
        var entity = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.Add(entity);
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
        
        layerManager.Remove(entity);

        Assert.That(stack.Entities, Does.Not.Contain(entity));
    }

    [Test]
    public void Remove_RemovesStack_IfNothingIsLeftInStack()
    {
        var entity = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.Add(entity);
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
        
        layerManager.Remove(entity);

        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
    }

    [Test]
    public void UpdatePosition_RemovesEntityFromStackPreviousPosition()
    {
        var mockPositionProvider = new Mock<IPositionProvider>();
        var entity = GetMockEntity(mockPositionProvider: mockPositionProvider);
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.Add(entity);
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
        mockPositionProvider.Setup(p => p.Position).Returns(HexVector.North);
        
        layerManager.UpdatePosition(entity, HexVector.Zero);
        
        Assert.That(layerManager.Stacks.TryGetValue(HexVector.Zero.GetHashCode(), out _), Is.False);
    }
    
    [Test]
    public void UpdatePosition_AddsEntityToStackAtItsPosition()
    {
        var entity = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);

        layerManager.UpdatePosition(entity, default);
        
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
    }

    [Test]
    public void ChangeLayer_RemovesEntityAtItsPosition()
    {
        var entity = GetMockEntity();
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.Add(entity);
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Entities, Does.Contain(entity));
        
        layerManager.ChangeLayer(entity, 0);
        
        Assert.That(layerManager.Stacks.TryGetValue(entity.PositionProvider.Position.GetHashCode(), out _), Is.False);
    }

    [Test]
    public void ChangeLayer_CallsRemove_FromIEntityManager()
    {
        var entity = GetMockEntity();
        
        layerManager.ChangeLayer(entity, 0);
        
        mockEntityManager.Verify(manager => manager.Remove(entity, 0), Times.Once);
    }
    
    [Test]
    public void ChangeLayer_CallsAdd_FromIEntityManager()
    {
        var entity = GetMockEntity();
        
        layerManager.ChangeLayer(entity, 0);
        
        mockEntityManager.Verify(manager => manager.Add(entity, 0), Times.Once);
    }

    [Test]
    public void GetHexHeight_ReturnsLayerHeight_IfNoHexOrStack()
    { 
        layerManager = new EntityLayerManager(1, 
            mockHexProvider.Object, 
            mockEntityManager.Object);
        var hexData = new Map.Properties(0, 0, 1f);
        
        var height = layerManager.GetHexHeight(HexVector.Zero);
        
        Assert.That(height, Is.EqualTo(1f));
    }
    
    [Test]
    public void GetHexHeight_ReturnsLayerAndHexHeight_IfNoStack()
    { 
        layerManager = new EntityLayerManager(1, 
            mockHexProvider.Object, 
            mockEntityManager.Object);
        var hexData = new Map.Properties(0, 0, 1f);
        var hex = new Hex(0, 0, new Properties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(HexVector.Zero)).Returns(hex);
        
        var height = layerManager.GetHexHeight(HexVector.Zero);
        
        Assert.That(height, Is.EqualTo(2f));
    }
    
    [Test]
    public void GetHexHeight_ReturnsLayerHexAndStackHeight_IfAllExist()
    { 
        layerManager = new EntityLayerManager(1, 
            mockHexProvider.Object, 
            mockEntityManager.Object);
        var hexData = new Map.Properties(0, 0, 1f);
        var hex = new Hex(0, 0, new Properties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(HexVector.Zero)).Returns(hex);
        var entity = GetMockEntity(heightData: new HeightData(1, 0));
        layerManager.Add(entity);
        
        var height = layerManager.GetHexHeight(HexVector.Zero);
        
        Assert.That(height, Is.EqualTo(3f));
    }
    
    [Test]
    public void GetHexHeight_ReturnsLayerHexAndStackHeight_IfAllExistAndExcludingEntity()
    { 
        layerManager = new EntityLayerManager(1, 
            mockHexProvider.Object, 
            mockEntityManager.Object);
        var hexData = new Map.Properties(0, 0, 1f);
        var hex = new Hex(0, 0, new Properties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(HexVector.Zero)).Returns(hex);
        var entity = GetMockEntity(heightData: new HeightData(1, 0));
        layerManager.Add(entity);
        
        var height = layerManager.GetHexHeight(HexVector.Zero, [entity]);
        
        Assert.That(height, Is.EqualTo(2f));
    }
    
    [Test]
    public void GetHexHeight_DoesNotThrow_IfNullExclude()
    { 
        layerManager = new EntityLayerManager(1, 
            mockHexProvider.Object, 
            mockEntityManager.Object);
        var hexData = new Map.Properties(0, 0, 1f);
        var hex = new Hex(0, 0, new Properties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(HexVector.Zero)).Returns(hex);
        var entity = GetMockEntity(heightData: new HeightData(1, 0));
        layerManager.Add(entity);
        
        Assert.DoesNotThrow(() => layerManager.GetHexHeight(HexVector.Zero, null));
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