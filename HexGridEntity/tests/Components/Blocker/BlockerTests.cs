namespace HexGrid.Entity.Tests.Components.Blocker;

using HexGrid.Entity.Components.Blocker;
using Map.Vector;

[TestFixture]
public class BlockerTests
{
    private Blocker blocker;
    private List<HexVector> blockedDirections;
    
    [SetUp]
    public void SetUp()
    {
        var entity = new Entity();
        blockedDirections = [HexVector.Forward];
        blocker = new Blocker(entity, blockedDirections);
    }

    [Test]
    public void Blocks_ReturnsFalseWhenEnabledIsFalseAndWouldNotBlock()
    {
        blocker.Disable();

        var blocks = blocker.Blocks(HexVector.Zero);
        
        Assert.That(blocks, Is.False);
    }
    
    [Test]
    public void Blocks_ReturnsFalseWhenEnabledIsFalseAndWouldBlock()
    {
        blocker.Disable();

        var blocks = blocker.Blocks(HexVector.Forward);
        
        Assert.That(blocks, Is.False);
    }

    [Test]
    public void Blocks_ReturnsTrueWhenEnabledAndWouldBlock()
    {
        blocker.Enable();

        var blocks = blocker.Blocks(HexVector.Forward);
        
        Assert.That(blocks, Is.True);
    }
    
    [Test]
    public void Blocks_ReturnsFalseWhenEnabledAndWouldNotBlock()
    {
        blocker.Enable();

        var blocks = blocker.Blocks(HexVector.Zero);
        
        Assert.That(blocks, Is.False);
    }
}