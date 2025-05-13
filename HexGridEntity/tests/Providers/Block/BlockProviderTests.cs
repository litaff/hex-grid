namespace HexGrid.Entity.Tests.Providers.Block;

using HexGrid.Entity.Providers.Block;
using Map.Vector;

[TestFixture]
public class BlockProviderTests
{
    private BlockProvider provider;
    
    [Test]
    public void ParameterlessConstructor_InitializesBlockedDirections()
    {
        provider = new BlockProvider();
        
        Assert.That(provider.BlockedDirections, Is.Not.Null);
        Assert.That(provider.BlockedDirections, Is.Empty);
    }

    [Test]
    public void Constructor_AssignsBlockedDirections()
    {
        List<HexVector> directions = [HexVector.Zero];
        
        provider = new BlockProvider(directions);
        
        Assert.That(provider.BlockedDirections, Is.EquivalentTo(directions));
    }

    [Test]
    public void Blocks_ReturnsTrue_WhenDirectionIsInBlockedDirections()
    {
        List<HexVector> directions = [HexVector.Zero];
        
        provider = new BlockProvider(directions);
        
        Assert.That(provider.Blocks(HexVector.Zero), Is.True);
    }
    
    [Test]
    public void Blocks_ReturnsFalse_WhenDirectionIsNotInBlockedDirections()
    {
        provider = new BlockProvider();
        
        Assert.That(provider.Blocks(HexVector.Zero), Is.False);
    }
}