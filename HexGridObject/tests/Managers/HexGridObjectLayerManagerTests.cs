namespace HexGridObject.Tests.Managers;

using global::HexGridObject.Handlers.Rotation;
using global::HexGridObject.Handlers.Translation;
using global::HexGridObject.Managers;
using global::HexGridObject.Providers.Position;
using global::HexGridObject.Providers.Rotation;
using HexGridMap;
using HexGridMap.Hex;
using HexGridMap.Vector;
using Moq;

[TestFixture]
public class HexGridObjectLayerManagerTests
{
    private HexGridObjectLayerManager layerManager;
    private Mock<IHexProvider> mockHexProvider;
    private Mock<IHexGridObjectManager> mockObjectManager;

    [SetUp]
    public void Setup()
    {
        mockHexProvider = new Mock<IHexProvider>();
        mockObjectManager = new Mock<IHexGridObjectManager>();
        layerManager = new HexGridObjectLayerManager(0, 
            mockHexProvider.Object, 
            mockObjectManager.Object);
    }

    [Test]
    public void AddGridObject_AddsANewStackWithObject_IfThereWasNoStackAtPosition()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);

        layerManager.AddGridObject(hexGridObject);
        
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
    }
    
    [Test]
    public void AddGridObject_AddsObjectToExistingStack_IfThereWasStackAtPosition()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.AddGridObject(hexGridObject);
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        hexGridObject = GetMockObject();
        
        layerManager.AddGridObject(hexGridObject);
        
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
    }

    [Test]
    public void RemoveGridObject_Returns_IfNoStackAtPosition()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);

        layerManager.RemoveGridObject(hexGridObject);
        
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
    }

    [Test]
    public void RemoveGridObject_RemovesObject_IfStackExistsAtPosition()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.AddGridObject(hexGridObject);
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        
        layerManager.RemoveGridObject(hexGridObject);

        Assert.That(stack.Objects, Does.Not.Contain(hexGridObject));
    }

    [Test]
    public void RemoveGridObject_RemovesStack_IfNothingIsLeftInStack()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.AddGridObject(hexGridObject);
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        
        layerManager.RemoveGridObject(hexGridObject);

        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
    }

    [Test]
    public void UpdateGridObjectPosition_RemovesObjectFromStackPreviousPosition()
    {
        var mockPositionProvider = new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(CubeHexVector.Zero);
        var mockRotationProvider = new Mock<IRotationProvider>();
        var mockTranslationHandler = new Mock<ITranslationHandler>();
        var mockRotationHandler = new Mock<IRotationHandler>();
        var heightData = new HeightData(0, 0);
        var hexGridObject = new HexGridObject(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, heightData);
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.AddGridObject(hexGridObject);
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        mockPositionProvider.Setup(p => p.Position).Returns(CubeHexVector.North);
        
        layerManager.UpdateGridObjectPosition(hexGridObject, CubeHexVector.Zero);
        
        Assert.That(layerManager.Stacks.TryGetValue(CubeHexVector.Zero.GetHashCode(), out _), Is.False);
    }
    
    [Test]
    public void UpdateGridObjectPosition_AddsObjectToStackAtItsPosition()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);

        layerManager.UpdateGridObjectPosition(hexGridObject, default);
        
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
    }

    [Test]
    public void ChangeGridObjectLayer_RemovesObjectAtItsPosition()
    {
        var hexGridObject = GetMockObject();
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
        layerManager.AddGridObject(hexGridObject);
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out var stack), Is.True);
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        
        layerManager.ChangeGridObjectLayer(hexGridObject, 0);
        
        Assert.That(layerManager.Stacks.TryGetValue(hexGridObject.PositionProvider.Position.GetHashCode(), out _), Is.False);
    }

    [Test]
    public void ChangeGridObjectLayer_CallsRemoveGridObject_FromIHexGridObjectManager()
    {
        var hexGridObject = GetMockObject();
        
        layerManager.ChangeGridObjectLayer(hexGridObject, 0);
        
        mockObjectManager.Verify(manager => manager.RemoveGridObject(hexGridObject, 0), Times.Once);
    }
    
    [Test]
    public void ChangeGridObjectLayer_CallsAddGridObject_FromIHexGridObjectManager()
    {
        var hexGridObject = GetMockObject();
        
        layerManager.ChangeGridObjectLayer(hexGridObject, 0);
        
        mockObjectManager.Verify(manager => manager.AddGridObject(hexGridObject, 0), Times.Once);
    }

    [Test]
    public void GetHexHeight_ReturnsLayerHeight_IfNoHexOrStack()
    { 
        layerManager = new HexGridObjectLayerManager(1, 
            mockHexProvider.Object, 
            mockObjectManager.Object);
        var hexData = new HexGridProperties(0, 0, 1f);
        
        var height = layerManager.GetHexHeight(CubeHexVector.Zero);
        
        Assert.That(height, Is.EqualTo(1f));
    }
    
    [Test]
    public void GetHexHeight_ReturnsLayerAndHexHeight_IfNoStack()
    { 
        layerManager = new HexGridObjectLayerManager(1, 
            mockHexProvider.Object, 
            mockObjectManager.Object);
        var hexData = new HexGridProperties(0, 0, 1f);
        var hex = new CubeHex(0, 0, new HexProperties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(CubeHexVector.Zero)).Returns(hex);
        
        var height = layerManager.GetHexHeight(CubeHexVector.Zero);
        
        Assert.That(height, Is.EqualTo(2f));
    }
    
    [Test]
    public void GetHexHeight_ReturnsLayerHexAndStackHeight_IfAllExist()
    { 
        layerManager = new HexGridObjectLayerManager(1, 
            mockHexProvider.Object, 
            mockObjectManager.Object);
        var hexData = new HexGridProperties(0, 0, 1f);
        var hex = new CubeHex(0, 0, new HexProperties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(CubeHexVector.Zero)).Returns(hex);
        var mockPositionProvider = new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(CubeHexVector.Zero);
        var mockRotationProvider = new Mock<IRotationProvider>();
        var mockTranslationHandler = new Mock<ITranslationHandler>();
        var mockRotationHandler = new Mock<IRotationHandler>();
        var heightData = new HeightData(1f, 0);
        var gridObject = new HexGridObject(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, heightData);
        layerManager.AddGridObject(gridObject);
        
        var height = layerManager.GetHexHeight(CubeHexVector.Zero);
        
        Assert.That(height, Is.EqualTo(3f));
    }
    
    [Test]
    public void GetHexHeight_ReturnsLayerHexAndStackHeight_IfAllExistAndExcludingObject()
    { 
        layerManager = new HexGridObjectLayerManager(1, 
            mockHexProvider.Object, 
            mockObjectManager.Object);
        var hexData = new HexGridProperties(0, 0, 1f);
        var hex = new CubeHex(0, 0, new HexProperties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(CubeHexVector.Zero)).Returns(hex);
        var mockPositionProvider = new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(CubeHexVector.Zero);
        var mockRotationProvider = new Mock<IRotationProvider>();
        var mockTranslationHandler = new Mock<ITranslationHandler>();
        var mockRotationHandler = new Mock<IRotationHandler>();
        var heightData = new HeightData(1f, 0);
        var gridObject = new HexGridObject(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, heightData);
        layerManager.AddGridObject(gridObject);
        
        var height = layerManager.GetHexHeight(CubeHexVector.Zero, [gridObject]);
        
        Assert.That(height, Is.EqualTo(2f));
    }
    
    [Test]
    public void GetHexHeight_DoesNotThrow_IfNullExclude()
    { 
        layerManager = new HexGridObjectLayerManager(1, 
            mockHexProvider.Object, 
            mockObjectManager.Object);
        var hexData = new HexGridProperties(0, 0, 1f);
        var hex = new CubeHex(0, 0, new HexProperties(1f), default);
        mockHexProvider.Setup(p => p.GetHex(CubeHexVector.Zero)).Returns(hex);
        var mockPositionProvider = new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(CubeHexVector.Zero);
        var mockRotationProvider = new Mock<IRotationProvider>();
        var mockTranslationHandler = new Mock<ITranslationHandler>();
        var mockRotationHandler = new Mock<IRotationHandler>();
        var heightData = new HeightData(1f, 0);
        var gridObject = new HexGridObject(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, heightData);
        layerManager.AddGridObject(gridObject);
        
        Assert.DoesNotThrow(() => layerManager.GetHexHeight(CubeHexVector.Zero, null));
    }
    
    private HexGridObject GetMockObject()
    {
        var mockPositionProvider = new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(CubeHexVector.Zero);
        var mockRotationProvider = new Mock<IRotationProvider>();
        var mockTranslationHandler = new Mock<ITranslationHandler>();
        var mockRotationHandler = new Mock<IRotationHandler>();
        var heightData = new HeightData(0, 0);
        return new HexGridObject(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, heightData);
    }
}