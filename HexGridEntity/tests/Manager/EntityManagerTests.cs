namespace HexGrid.Entity.Tests.Manager;

using HexGrid.Entity.Manager;
using HexGrid.Entity.Providers;
using Moq;

[TestFixture]
public class EntityManagerTests
{
    private EntityManager manager;
    private Mock<IHexStateProvider> mockHexStateProvider;
    private Mock<IEntityStorage> mockEntityStorage;

    [SetUp]
    public void Setup()
    {
        mockEntityStorage = new Mock<IEntityStorage>();
        mockHexStateProvider = new Mock<IHexStateProvider>();
        manager = new EntityManager(mockHexStateProvider.Object, mockEntityStorage.Object);
    }

    [Test]
    public void Add_AddsEntityToEntityStorage_AtEntityPosition()
    {
        var mockEntity = new MockEntity();
    
        manager.Add(mockEntity.Entity);
        
        mockEntityStorage.Verify(storage => storage.Add(mockEntity.Entity, mockEntity.Entity.GridPosition), Times.Once);
    }
    
    [Test]
    public void Add_EnablesAddedEntity()
    {
        var mockEntity = new MockEntity();
    
        manager.Add(mockEntity.Entity);
        
        Assert.That(mockEntity.Entity.Enabled, Is.True);
    }
    
    [Test]
    public void Remove_RemovesEntityFromEntityStorage_AtEntityPosition()
    {
        var mockEntity = new MockEntity();
    
        manager.Remove(mockEntity.Entity);
        
        mockEntityStorage.Verify(storage => storage.Remove(mockEntity.Entity, mockEntity.Entity.GridPosition), Times.Once);
    }
    
    [Test]
    public void Remove_DisablesRemovedEntity()
    {
        var mockEntity = new MockEntity();
    
        manager.Remove(mockEntity.Entity);
        
        Assert.That(mockEntity.Entity.Enabled, Is.False);
    }
}