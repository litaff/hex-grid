namespace hex_grid.scripts.hex_grid.hex;

public class AccessibleHex : CubeHex
{
    public override HexType Type => HexType.Accessible;
    
    public AccessibleHex(float size, int libraryIndex) : base(size, libraryIndex)
    {
    }

    public AccessibleHex(int q, int r, float size, int libraryIndex) : base(q, r, size, libraryIndex)
    {
    }
}