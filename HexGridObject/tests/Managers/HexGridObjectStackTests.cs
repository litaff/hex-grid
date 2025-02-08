namespace HexGridObject.Tests.Managers;

using global::HexGridObject.Managers;
using global::HexGridObject.Providers.Position;
using global::HexGridObject.Providers.Translation;
using Moq;

[TestFixture]
public class HexGridObjectStackTests
{
    private HexGridObject hexGridObject;
    private HexGridObjectStack stack;

    [SetUp]
    public void Setup()
    {
        hexGridObject = GetMockObject();
        stack = new HexGridObjectStack(hexGridObject);
    }

    [Test]
    public void Add_AddsObject_IfNotPresent()
    {
        stack = new HexGridObjectStack();
        Assert.That(stack.Objects, Does.Not.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(0));

        stack.Add(hexGridObject);
        
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(1));
    }

    [Test]
    public void Add_Returns_IfObjectPresent()
    {
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(1));
        
        stack.Add(hexGridObject);
        
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(1));
    }

    [Test]
    public void Remove_RemovesObject_IfPresent()
    {
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(1));
        
        stack.Remove(hexGridObject);
    
        Assert.That(stack.Objects, Does.Not.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(0));
    }

    [Test]
    public void Remove_DoesNothing_IfObjectNotPresent()
    {
        stack = new HexGridObjectStack();
        Assert.That(stack.Objects, Does.Not.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(0));

        stack.Remove(hexGridObject);
        
        Assert.That(stack.Objects, Does.Not.Contain(hexGridObject));
        Assert.That(stack.Objects, Has.Count.EqualTo(0));
    }

    [TestCase(1)]
    [TestCase(0.9f)]
    [TestCase(0)]
    [TestCase(-0.9f)]
    [TestCase(-1)]
    [TestCase(-1, 1, 10, -1.9f)]
    public void GetStackHeight_ReturnsCorrectHeight(params float[] heights)
    {
        stack = new HexGridObjectStack();
        foreach (var height in heights)
        {
            stack.Add(GetMockObject(height));
        }
        Assert.That(stack.Objects, Has.Count.EqualTo(heights.Length));

        var stackHeight = stack.GetStackHeight();
        
        Assert.That(stackHeight, Is.EqualTo(heights.Sum()));
    }

    [Test]
    public void Contains_ReturnsTrue_IfTypeIsPresent()
    {
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        Assert.That(hexGridObject.GetType(), Is.EqualTo(typeof(HexGridObject)));

        var contains = stack.Contains<HexGridObject>();
        
        Assert.That(contains, Is.True);
    }
    
    [Test]
    public void Contains_ReturnsFalse_IfTypeIsNotPresent()
    {
        Assert.That(stack.Objects, Does.Contain(hexGridObject));
        Assert.That(hexGridObject.GetType(), Is.EqualTo(typeof(HexGridObject)));

        var contains = stack.Contains<int>();
        
        Assert.That(contains, Is.False);
    }

    private HexGridObject GetMockObject(float height = 0)
    {
        var mockPositionProvider = new Mock<IHexGridPositionProvider>();
        var mockTranslationProvider = new Mock<ITranslationProvider>();
        var heightData = new HeightData(height, 0);
        return new HexGridObject(mockPositionProvider.Object, mockTranslationProvider.Object, heightData);
    }
}