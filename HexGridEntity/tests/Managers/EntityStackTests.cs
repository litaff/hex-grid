namespace HexGrid.Entity.Tests.Managers;

using HexGrid.Entity;
using HexGrid.Entity.Handlers.Position;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Managers;
using HexGrid.Entity.Providers.Block;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using Map.Vector;
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
            stack.Add(GetMockEntity(heightData: new HeightData(height, 0)));
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
            stack.Add(GetMockEntity(heightData: new HeightData(height, 0)));
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

    private Entity GetMockEntity(
        Mock<IPositionProvider>? mockPositionProvider = null,
        Mock<IRotationProvider>? mockRotationProvider = null,
        Mock<IPositionHandler>? mockTranslationHandler = null,
        Mock<IRotationHandler>? mockRotationHandler = null,
        Mock<IBlockProvider>? mockBlockProvider = null,
        HeightData heightData = new())
    {
        mockPositionProvider ??= new Mock<IPositionProvider>();
        mockPositionProvider.Setup(p => p.Position).Returns(HexVector.Zero);
        mockRotationProvider ??= new Mock<IRotationProvider>();
        mockTranslationHandler ??= new Mock<IPositionHandler>();
        mockRotationHandler ??= new Mock<IRotationHandler>();
        mockBlockProvider ??= new Mock<IBlockProvider>();
        return new Entity(mockPositionProvider.Object, mockRotationProvider.Object, mockTranslationHandler.Object, mockRotationHandler.Object, mockBlockProvider.Object, heightData);
    }
}