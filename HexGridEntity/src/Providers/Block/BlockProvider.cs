namespace HexGrid.Entity.Providers.Block;

using Map.Vector;

public class BlockProvider : IBlockProvider
{
    public List<HexVector> BlockedDirections { get; }

    public BlockProvider()
    {
        BlockedDirections = [];
    }
    
    public BlockProvider(List<HexVector> blockedDirections)
    {
        BlockedDirections = blockedDirections;
    }

    public bool Blocks(HexVector direction)
    {
        return BlockedDirections.Contains(direction);
    }
}