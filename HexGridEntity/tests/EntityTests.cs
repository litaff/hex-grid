namespace HexGrid.Entity.Tests;

[TestFixture]
public class EntityTests
{
    private Entity entity;

    [SetUp]
    public void SetUp()
    {
        entity = new Entity();
    }

    [Test]
    public void AddComponent_AssignsComponentAtType()
    {
        var component = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
        
        entity.AddComponent(component);
        
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.True);
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(component));
    }

    [Test]
    public void AddComponent_OverridesExistingComponent()
    {
        var component = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
        entity.AddComponent(component);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.True);
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(component));
        var overrideComponent = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(overrideComponent.GetType()), Is.True);
        
        entity.AddComponent(overrideComponent);
        
        Assert.That(entity.Components.ContainsKey(overrideComponent.GetType()), Is.True);
        Assert.That(entity.Components[overrideComponent.GetType()], Is.EqualTo(overrideComponent));
    }

    [Test]
    public void RemoveComponent_DoesNotThrowWhenNoComponentFound()
    {
        var absentComponent = new MockComponent(entity);
        Assert.DoesNotThrow(() => entity.RemoveComponent(absentComponent));
    }

    [Test]
    public void RemoveComponent_RemovesExistingComponent()
    {
        var component = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
        entity.AddComponent(component);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.True);
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(component));
        
        entity.RemoveComponent(component);
        
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
    }

    [Test]
    public void GetComponent_ReturnsPresentComponentOfType()
    {
        var component = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
        entity.AddComponent(component);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.True);
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(component));

        var getComponent = entity.GetComponent<MockComponent>();
        
        Assert.That(getComponent, Is.Not.Null);
        Assert.That(getComponent.GetType(), Is.EqualTo(component.GetType()));
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(getComponent));
    }
    
    [Test]
    public void GetComponent_ReturnsNullWhenComponentNotFound()
    {
        Assert.That(entity.Components.ContainsKey(typeof(MockComponent)), Is.False);

        var getComponent = entity.GetComponent<MockComponent>();
        
        Assert.That(getComponent, Is.Null);
    }

    [Test]
    public void Enable_AssignsEnabledTrue()
    {
        Assert.That(entity.Enabled, Is.False);
        
        entity.Enable();
        
        Assert.That(entity.Enabled, Is.True);
    }

    [Test]
    public void Enable_CallsEnableOnComponents()
    {
        var component = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
        entity.AddComponent(component);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.True);
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(component));
        var enableCalled = false;
        component.EnableCalled += () => enableCalled = true;
        
        entity.Enable();
        
        Assert.That(enableCalled, Is.True);
    }
    
    [Test]
    public void Disable_AssignsEnableFalse()
    {
        Assert.That(entity.Enabled, Is.False);
        entity.Enable();
        Assert.That(entity.Enabled, Is.True);
        
        entity.Disable();
        
        Assert.That(entity.Enabled, Is.False);
    }

    [Test]
    public void Disable_CallsDisableOnComponents()
    {
        var component = new MockComponent(entity);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.False);
        entity.AddComponent(component);
        Assert.That(entity.Components.ContainsKey(component.GetType()), Is.True);
        Assert.That(entity.Components[component.GetType()], Is.EqualTo(component));
        var disableCalled = false;
        component.DisableCalled += () => disableCalled = true;
        
        entity.Disable();
        
        Assert.That(disableCalled, Is.True);
    }
}