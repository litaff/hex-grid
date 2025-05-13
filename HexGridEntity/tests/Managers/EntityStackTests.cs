namespace HexGrid.Entity.Tests.Managers;

using HexGrid.Entity;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Handlers.Translation;
using HexGrid.Entity.Managers;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using Moq;

[TestFixture]
public class EntityStackTests
{
    private Entity entity;
    private EntityStack stack;

    [SetUp]
    public void Setup()
    {
        entity = GetMockEntity();
        stack = new EntityStack(entity);
    }

    [Test]
    public void Add_AddsEntity_IfNotPresent()
    {
        stack = new EntityStack();
        Assert.That(stack.Entities, Does.Not.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(0));

        stack.Add(entity);
        
        Assert.That(stack.Entities, Does.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(1));
    }

    [Test]
    public void Add_Returns_IfEntityPresent()
    {
        Assert.That(stack.Entities, Does.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(1));
        
        stack.Add(entity);
        
        Assert.That(stack.Entities, Does.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(1));
    }

    [Test]
    public void Remove_RemovesEntity_IfPresent()
    {
        Assert.That(stack.Entities, Does.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(1));
        
        stack.Remove(entity);
    
        Assert.That(stack.Entities, Does.Not.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(0));
    }

    [Test]
    public void Remove_DoesNothing_IfEntityNotPresent()
    {
        stack = new EntityStack();
        Assert.That(stack.Entities, Does.Not.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(0));

        stack.Remove(entity);
        
        Assert.That(stack.Entities, Does.Not.Contain(entity));
        Assert.That(stack.Entities, Has.Count.EqualTo(0));
    }

    [TestCase(1)]
    [TestCase(0.9f)]
    [TestCase(0)]
    [TestCase(-0.9f)]
    [TestCase(-1)]
    [TestCase(-1, 1, 10, -1.9f)]
    public void GetStackHeight_ReturnsCorrectHeight(params float[] heights)
    {
        stack = new EntityStack();
        foreach (var height in heights)
        {
            stack.Add(GetMockEntity(height));
        }
        Assert.That(stack.Entities, Has.Count.EqualTo(heights.Length));

        var stackHeight = stack.GetStackHeight();
        
        Assert.That(stackHeight, Is.EqualTo(heights.Sum()));
    }
    
    [TestCase(1)]
    [TestCase(0.9f)]
    [TestCase(0)]
    [TestCase(-0.9f)]
    [TestCase(-1)]
    [TestCase(-1, 1, 10, -1.9f)]
    public void GetStackHeight_ReturnsCorrectHeight_WithExcludes(params float[] heights)
    {
        stack = new EntityStack();
        foreach (var height in heights)
        {
            stack.Add(GetMockEntity(height));
        }
        Assert.That(stack.Entities, Has.Count.EqualTo(heights.Length));
        var exclude = stack.Entities[0];
        
        var stackHeight = stack.GetStackHeight([exclude]);
        
        Assert.That(stackHeight, Is.EqualTo(heights.Sum() - exclude.HeightData.Height));
    }

    [Test]
    public void Contains_ReturnsTrue_IfTypeIsPresent()
    {
        Assert.That(stack.Entities, Does.Contain(entity));
        Assert.That(entity.GetType(), Is.EqualTo(typeof(Entity)));

        var contains = stack.Contains<Entity>();
        
        Assert.That(contains, Is.True);
    }
    
    [Test]
    public void Contains_ReturnsFalse_IfTypeIsNotPresent()
    {
        Assert.That(stack.Entities, Does.Contain(entity));
        Assert.That(entity.GetType(), Is.EqualTo(typeof(Entity)));

        var contains = stack.Contains<int>();
        
        Assert.That(contains, Is.False);
    }

    private Entity GetMockEntity(float height = 0)
    {
        var mockPositionProvider = new Mock<IPositionProvider>();
        var mockTranslationHandler = new Mock<ITranslationHandler>();
        var mockRotationProvider = new Mock<IRotationProvider>();
        var mockRotationHandler = new Mock<IRotationHandler>();
        var heightData = new HeightData(height, 0);
        return new Entity(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, heightData);
    }
}