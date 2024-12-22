namespace HexGridObject.Tests.Managers;

using global::HexGridObject.Managers;
using global::HexGridObject.Providers.Position;
using global::HexGridObject.Providers.Translation.Providers;
using HexGridMap.Interfaces;
using Moq;

[TestFixture]
public class HexGridObjectManagerTests
{
    private HexGridObjectManager manager;

    [SetUp]
    public void Setup()
    {
        var hexProviders = new Dictionary<int, IHexProvider>
        {
            { 0, new Mock<IHexProvider>().Object }
        };
        manager = new HexGridObjectManager(hexProviders);
    }

    [Test]
    public void AddGridObject_Returns_IfNoLayerManagerAtLayer()
    {
        var gridObject = GetMockObject();
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
        
        manager.AddGridObject(gridObject, 1);
        
        // Checks if layer manager wasn't added, but should be ok.
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
    }

    [Test]
    public void AddGridObject_CallsAddGridObject_FromLayerManager()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void AddGridObject_CallsEnable_FromObject()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void AddGridObject_CallsRegisterHandlers_FromObject()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void RemoveGridObject_Returns_IfNoLayerManagerAtLayer()
    {
        var gridObject = GetMockObject();
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
        
        manager.RemoveGridObject(gridObject, 1);
        
        // Checks if layer manager wasn't added, but should be ok.
        Assert.That(manager.LayerManagers.TryGetValue(1, out _), Is.False);
    }

    [Test]
    public void RemoveGridObject_CallsRemoveGridObject_FromLayerManager()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void RemoveGridObject_CallsDisable_FromObject()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    [Test]
    public void RemoveGridObject_CallsUnregisterHandlers_FromObject()
    {
        Assert.Inconclusive("Can not check if method was called.");
    }
    
    private HexGridObject GetMockObject()
    {
        var mockPositionProvider = new Mock<IHexGridPositionProvider>();
        var mockTranslationProvider = new Mock<ITranslationProvider>();
        var heightData = new HeightData(0, 0);
        return new HexGridObject(mockPositionProvider.Object, mockTranslationProvider.Object, heightData);
    }
}