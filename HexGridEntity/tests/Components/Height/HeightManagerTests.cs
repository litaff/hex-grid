namespace HexGrid.Entity.Tests.Components.Height;

using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Managers;
using Map.Vector;
using Moq;

[TestFixture]
public class HeightManagerTests
{
    private HeightManager heightManager;
    private Mock<IEntityManager> mockEntityManager;

    [SetUp]
    public void SetUp()
    {
        mockEntityManager = new Mock<IEntityManager>();

        heightManager = new HeightManager(mockEntityManager.Object);
    }

    [Test]
    public void GetHeight_Returns0WhenNoEntities()
    {
        var position = HexVector.Zero;
        var stack = new EntityStack();
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(position.GetHashCode(), out stack)).Returns(false);

        var height = heightManager.GetHeight(position);
        
        Assert.That(height, Is.EqualTo(0));
    }
    
    [Test]
    public void GetHeight_ReturnsSumOfEntitiesHeightComponents()
    {
        var position = HexVector.Zero;
        var entity = new Entity();
        const int entityHeight = 1;
        var heightComponent = new Height(entity, entityHeight);
        entity.AddComponent(heightComponent);
        heightComponent.Enable();
        var stack = new EntityStack(entity);
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(position.GetHashCode(), out stack)).Returns(true);

        var height = heightManager.GetHeight(position);
        
        Assert.That(height, Is.EqualTo(entityHeight));
    }
    
    [Test]
    public void GetHeight_ReturnsSumOfEntitiesHeightComponentsWithoutExcludedEntities()
    {
        var position = HexVector.Zero;
        var entity = new Entity();
        const int entityHeight = 1;
        var heightComponent = new Height(entity, entityHeight);
        entity.AddComponent(heightComponent);
        heightComponent.Enable();
        var stack = new EntityStack(entity);
        mockEntityManager.Setup(manager => manager.Stacks.TryGetValue(position.GetHashCode(), out stack)).Returns(true);

        var height = heightManager.GetHeight(position, [entity]);
        
        Assert.That(height, Is.EqualTo(0));
    }
}