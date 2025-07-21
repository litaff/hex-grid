namespace HexGrid.Entity.Tests.Manager;

using HexGrid.Entity.Manager;
using Map.Vector;

[TestFixture]
public class EntityStorageTests
{
    private EntityStorage entityStorage;
    
    [SetUp]
    public void SetUp()
    {
        entityStorage = new EntityStorage();
    }

    [Test]
    public void Add_CreatesAStackAtPosition_IfNoExists()
    {
        var mockEntity = new MockEntity();
        var position = HexVector.Zero;
        mockEntity.MockPositionProvider.Setup(mock => mock.Position).Returns(position);
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.False);
        
        entityStorage.Add(mockEntity.Entity, position);
        
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.True);
    }
    
    [Test]
    public void Add_AddsEntityToExistingStack_IfExists()
    {
        var mockEntity = new MockEntity();
        var position = HexVector.Zero;
        mockEntity.MockPositionProvider.Setup(mock => mock.Position).Returns(position);
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.False);
        entityStorage.Add(mockEntity.Entity, position);
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.True);
        
        entityStorage.Add(new MockEntity().Entity, position);
        
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.True);
        Assert.That(entityStorage.Stacks[position.GetHashCode()].Entities, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void Remove_DoesNotThrow_IfNoStackAtPosition()
    {
        var position = HexVector.Zero;
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.False);
        
        Assert.DoesNotThrow(()=> entityStorage.Remove(new MockEntity().Entity, position));
        
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.False);
    }
    
    [Test]
    public void Remove_RemovesStack_IfEmpty()
    {
        var mockEntity = new MockEntity();
        var position = HexVector.Zero;
        mockEntity.MockPositionProvider.Setup(mock => mock.Position).Returns(position);
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.False);
        entityStorage.Add(mockEntity.Entity, position);
        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.True);
        
        entityStorage.Remove(mockEntity.Entity, position);

        Assert.That(entityStorage.Stacks.TryGetValue(position.GetHashCode(), out _), Is.False);
    }
}