namespace HexGrid.Entity.Providers.Block;

using Map.Vector;

public interface IBlockProvider
{
    public bool Blocks(HexVector direction);
}