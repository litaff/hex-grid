namespace HexGrid.Entity.Tests;

using Components;

[TestFixture]
public class EntityBlueprintTests
{
    private EntityBlueprint blueprint;

    [SetUp]
    public void SetUp()
    {
        blueprint = new EntityBlueprint();
    }

    [Test]
    public void Add_AddsDescriptorAtType()
    {
        var descriptor = new MockComponentDescriptor();
        Assert.That(blueprint.Descriptors.ContainsKey(descriptor.GetType()), Is.False);
        
        blueprint.Add(descriptor);
        
        Assert.That(blueprint.Descriptors.ContainsKey(descriptor.GetType()), Is.True);
    }

    [Test]
    public void Add_OverridesDescriptorAtType()
    {
        var descriptor = new MockComponentDescriptor();
        Assert.That(blueprint.Descriptors.ContainsKey(descriptor.GetType()), Is.False);
        blueprint.Add(descriptor);
        Assert.That(blueprint.Descriptors.ContainsKey(descriptor.GetType()), Is.True);
        var overrideDescriptor = new MockComponentDescriptor();
        
        blueprint.Add(overrideDescriptor);
        
        Assert.That(blueprint.Descriptors.ContainsKey(overrideDescriptor.GetType()), Is.True);
        Assert.That(blueprint.Descriptors[overrideDescriptor.GetType()], Is.Not.EqualTo(descriptor));
    }

    [Test]
    public void Get_ReturnsDescriptorOfType()
    {
        var descriptor = new MockComponentDescriptor();
        Assert.That(blueprint.Descriptors.ContainsKey(descriptor.GetType()), Is.False);
        blueprint.Add(descriptor);
        Assert.That(blueprint.Descriptors.ContainsKey(descriptor.GetType()), Is.True);

        var get = blueprint.Get<MockComponentDescriptor>();
        
        Assert.That(get, Is.EqualTo(descriptor));
    }
    
    [Test]
    public void Get_ThrowsWhenDescriptorNotFound()
    {
        Assert.That(blueprint.Descriptors.ContainsKey(typeof(MockComponentDescriptor)), Is.False);

        Assert.Throws<ArgumentNullException>(() => blueprint.Get<MockComponentDescriptor>());
    }
}