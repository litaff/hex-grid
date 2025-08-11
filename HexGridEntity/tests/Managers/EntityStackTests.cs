namespace HexGrid.Entity.Tests.Managers;

using HexGrid.Entity.Managers;

[TestFixture]
public class EntityStackTests
{
    private EntityStack entityStack;

    [SetUp]
    public void SetUp()
    {
        entityStack = new EntityStack();
    }

    [Test]
    public void Add_AddsEntity()
    {
        var entity = new Entity();
        Assert.That(entityStack.Entities, Does.Not.Contain(entity));
        
        entityStack.Add(entity);
        
        Assert.That(entityStack.Entities, Does.Contain(entity));
    }

    [Test]
    public void Add_DoesNotAddDuplicates()
    {
        var entity = new Entity();
        Assert.That(entityStack.Entities, Does.Not.Contain(entity));
        entityStack.Add(entity);
        Assert.That(entityStack.Entities, Does.Contain(entity));
        Assert.That(entityStack.Entities, Has.Count.EqualTo(1));
        
        entityStack.Add(entity);
        
        Assert.That(entityStack.Entities, Does.Contain(entity));
        Assert.That(entityStack.Entities, Has.Count.EqualTo(1));
    }

    [Test]
    public void Remove_DoesNotThrowWhenEntityNotFound()
    {
        var entity = new Entity();
        Assert.DoesNotThrow(() => entityStack.Remove(entity));
    }

    [Test]
    public void Remove_RemovesEntity()
    {
        var entity = new Entity();
        Assert.That(entityStack.Entities, Does.Not.Contain(entity));
        entityStack.Add(entity);
        Assert.That(entityStack.Entities, Does.Contain(entity));
        Assert.That(entityStack.Entities, Has.Count.EqualTo(1));
        
        entityStack.Remove(entity);
        
        Assert.That(entityStack.Entities, Does.Not.Contain(entity));
        Assert.That(entityStack.Entities, Is.Empty);
    }

    [Test]
    public void GetComponents_ReturnsComponentsOfTypeFromAllEntities()
    {
        var entityOne = new Entity();
        var componentOne = new MockComponent(entityOne);
        entityOne.AddComponent(componentOne);
        var entityTwo = new Entity();
        var componentTwo = new MockComponent(entityTwo);
        entityTwo.AddComponent(componentTwo);
        entityStack.Add(entityOne);
        entityStack.Add(entityTwo);
        Assert.That(entityStack.Entities, Has.Count.EqualTo(2));

        var components = entityStack.GetComponents<MockComponent>();
        
        Assert.That(components, Does.Contain(componentOne));
        Assert.That(components, Does.Contain(componentTwo));
    }
    
    [Test]
    public void GetComponents_ReturnsComponentsOfTypeFromAllNotExcludedEntities()
    {
        var entityOne = new Entity();
        var componentOne = new MockComponent(entityOne);
        entityOne.AddComponent(componentOne);
        var entityTwo = new Entity();
        var componentTwo = new MockComponent(entityTwo);
        entityTwo.AddComponent(componentTwo);
        entityStack.Add(entityOne);
        entityStack.Add(entityTwo);
        Assert.That(entityStack.Entities, Has.Count.EqualTo(2));

        var components = entityStack.GetComponents<MockComponent>([entityTwo]);
        
        Assert.That(components, Does.Contain(componentOne));
        Assert.That(components, Does.Not.Contain(componentTwo));
    }

    [Test]
    public void Contains_ReturnsTrueWhenStackContainsEntityType()
    {
        var entity = new Entity();
        entityStack.Add(entity);
        Assert.That(entityStack.Entities, Does.Contain(entity));
        
        Assert.That(entityStack.Contains<Entity>(), Is.True);
    }
    
    [Test]
    public void Contains_ReturnsFalseWhenStackDoesNotContainEntityType()
    {
        Assert.That(entityStack.Entities, Is.Empty);
        
        Assert.That(entityStack.Contains<Entity>(), Is.False);
    }

    [Test]
    public void IsEmpty_IsTrueWhenNoEntities()
    {
        Assert.That(entityStack.Entities, Is.Empty);
        
        Assert.That(entityStack.IsEmpty, Is.True);
    }
}