namespace HexGrid.Entity.Tests.Managers;

using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Transform;
using HexGrid.Entity.Managers;
using Map.Vector;
using Moq;

[TestFixture]
public class EntityManagerTests
{
    private EntityManager entityManager;
    
    [SetUp]
    public void SetUp()
    {
        entityManager = new EntityManager();
    }

    [Test]
    public void Add_DoesNotThrowWhenEntityHasNoTransform()
    {
        var entity = new Entity();
        Assert.That(entity.GetComponent<Transform>(), Is.Null);
        
        Assert.DoesNotThrow(() => entityManager.Add(entity));
    }

    [Test]
    public void Add_AddsEntityWithATransform()
    {
        var entity = new Entity();
        var position = HexVector.Zero;
        var transform = new Transform(entity, new Mock<IHexManager>().Object, new Mock<IBlockerManager>().Object,
            position: position);
        entity.AddComponent(transform);
        Assert.That(entity.GetComponent<Transform>(), Is.Not.Null);
        
        entityManager.Add(entity);
        
        Assert.That(entityManager.Stacks[position.GetHashCode()].Entities, Does.Contain(entity));
    }
    
    [Test]
    public void Remove_DoesNotThrowWhenEntityHasNoTransform()
    {
        var entity = new Entity();
        Assert.That(entity.GetComponent<Transform>(), Is.Null);
        
        Assert.DoesNotThrow(() => entityManager.Remove(entity));
    }

    [Test]
    public void Contains_DoesNotThrowWhenNoEntitiesAtPosition()
    {
        var position = HexVector.Zero;
        
        Assert.DoesNotThrow(() => entityManager.Contains<Entity>(position));
    }
    
    [Test]
    public void EntityMovesWhenTransformUpdatesPosition()
    {
        var entity = new Entity();
        var currentPosition = HexVector.Zero;
        var targetPosition = HexVector.Forward;
        var mockHexManager = new Mock<IHexManager>();
        var closest = targetPosition;
        mockHexManager.Setup(manager => manager.TryGetClosest(targetPosition, 0, out closest)).Returns(true);
        var mockBlockerManager = new Mock<IBlockerManager>();
        mockBlockerManager.Setup(manager =>
            manager.IsBlocked(closest, currentPosition.DirectionTo(closest).Normalized())).Returns(false);
        mockBlockerManager.Setup(manager => 
            manager.IsBlocked(currentPosition, closest.DirectionTo(currentPosition).Normalized())).Returns(false);
        var transform = new Transform(entity, mockHexManager.Object, mockBlockerManager.Object,
            position: currentPosition);
        entity.AddComponent(transform);
        entityManager.Add(entity);
        transform.Enable();
        
        transform.Position.Translate(targetPosition);

        Assert.That(entityManager.Contains<Entity>(currentPosition), Is.False);
        Assert.That(entityManager.Contains<Entity>(targetPosition), Is.True);
        Assert.That(entityManager.Stacks[targetPosition.GetHashCode()].Entities, Does.Contain(entity));
    }
}