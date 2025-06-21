namespace HexGrid.Entity.Tests.Providers.Rotation;

using HexGrid.Entity.Providers.Rotation;
using Map.Vector;

[TestFixture]
public class RotationProviderTests
{
    private RotationProvider provider;
    private HexVector initialRotation;
    
    [SetUp]
    public void Setup()
    {
        initialRotation = HexVector.Forward;
        provider = new RotationProvider(initialRotation);
    }

    [Test]
    public void Constructor_InitializesForward()
    {
        Assert.That(provider.Forward, Is.EqualTo(initialRotation));
    }
    
    [Test]
    public void Constructor_NormalizesInitialRotation()
    {
        var rotation = new HexVector(5, 5);

        provider = new RotationProvider(rotation);

        var normalizedRotation = rotation.Normalized();
        Assert.That(provider.Forward, Is.EqualTo(normalizedRotation));
    }

    [Test]
    public void RotateTowards_ReturnsFalse_WhenTargetIsEqualToForward()
    {
        Assert.That(provider.RotateTowards(initialRotation), Is.False);
    }
    
    [Test]
    public void RotateTowards_ReturnsTrue_WhenTargetIsNotEqualToForward()
    {
        Assert.That(provider.RotateTowards(HexVector.Backward), Is.True);
    }

    [Test]
    public void RotateTowards_NormalizesTarget()
    {
        var rotation = HexVector.Backward;
        provider.RotateTowards(rotation * 5);
        
        Assert.That(provider.Forward, Is.EqualTo(rotation));
    }

    [Test]
    public void RotateTowards_InvokeOnRotationChanged_WhenForwardChanges()
    {
        var called = false;
        provider.OnRotationChanged += () => called = true;

        provider.RotateTowards(HexVector.Backward);
        
        Assert.That(called, Is.True);
    }
    
    [Test]
    public void RotateTowards_DoesNotInvokeOnRotationChanged_WhenForwardDoesNotChange()
    {
        var called = false;
        provider.OnRotationChanged += () => called = true;

        provider.RotateTowards(initialRotation);
        
        Assert.That(called, Is.False);
    }
}