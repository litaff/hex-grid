namespace HexGrid.Entity.Tests.Components.Blocker;

using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Managers;
using Map.Vector;
using Moq;

[TestFixture]
public class BlockerManagerTests
{
    private BlockerManager blockerManager;
    private Mock<IEntityManager> mockEntityManager;
    
    [SetUp]
    public void SetUp()
    {
        mockEntityManager = new Mock<IEntityManager>();
        blockerManager = new BlockerManager(mockEntityManager.Object);
    }

    [Test]
    public void IsBlocked_ReturnsFalseWhenNoEntitiesAtTargetPosition()
    {
        var targetPosition = HexVector.Zero;
        var direction = HexVector.Zero;
        var stack = new EntityStack();
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(targetPosition.GetHashCode(), out stack)).Returns(false);

        var blocked = blockerManager.IsBlocked(targetPosition, direction);
        
        Assert.That(blocked, Is.False);
    }
    
    [Test]
    public void IsBlocked_ReturnsFalseWhenNoEntitiesWithBlockerComponentAtTargetPosition()
    {
        var targetPosition = HexVector.Zero;
        var direction = HexVector.Zero;
        var stack = new EntityStack(new Entity());
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(targetPosition.GetHashCode(), out stack)).Returns(true);

        var blocked = blockerManager.IsBlocked(targetPosition, direction);
        
        Assert.That(blocked, Is.False);
    }
    
    [Test]
    public void IsBlocked_ReturnsFalseWhenEntitiesWithBlockerComponentAtTargetPositionDoNotBlockDirection()
    {
        var targetPosition = HexVector.Zero;
        var direction = HexVector.Zero;
        var entity = new Entity();
        var blocker = new Blocker(entity, []);
        blocker.Enable();
        entity.AddComponent(blocker);
        var stack = new EntityStack(entity);
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(targetPosition.GetHashCode(), out stack)).Returns(true);

        var blocked = blockerManager.IsBlocked(targetPosition, direction);
        
        Assert.That(blocked, Is.False);
    }
    
    [Test]
    public void IsBlocked_ReturnsTrueWhenEntitiesWithBlockerComponentAtTargetPositionBlockDirection()
    {
        var targetPosition = HexVector.Zero;
        var direction = HexVector.Zero;
        var entity = new Entity();
        var blocker = new Blocker(entity, [direction]);
        blocker.Enable();
        entity.AddComponent(blocker);
        var stack = new EntityStack(entity);
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(targetPosition.GetHashCode(), out stack)).Returns(true);

        var blocked = blockerManager.IsBlocked(targetPosition, direction);
        
        Assert.That(blocked, Is.True);
    }
}