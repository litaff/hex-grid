namespace HexGrid.Entity.Tests.Providers.Position;

using HexGrid.Entity;
using HexGrid.Entity.Providers;
using HexGrid.Entity.Providers.Position;
using Map.Vector;
using Moq;

[TestFixture]
public class PositionProviderTests
{
    private PositionProvider provider;
    private HexVector initialPosition;
    private HeightData heightData;
    
    [SetUp]
    public void Setup()
    {
        initialPosition = HexVector.Zero;
        heightData = new HeightData(1, 1);
        provider = new PositionProvider(initialPosition, heightData);
    }
    
    [Test]
    public void CanTranslate_DoesNotThrow_IfNoProviders()
    {
        provider.Disable();

        Assert.DoesNotThrow(() => provider.CanTranslate(HexVector.Zero, out _));
    }

    [Test]
    public void Enable_InvokesOnPositionChanged_WithCurrentPosition()
    {
        var called = false;
        provider.OnPositionChanged += _ => called = true;
        
        provider.Enable(null);
        
        Assert.That(called, Is.True);
    }
}