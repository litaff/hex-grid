namespace hex_grid.scripts.hex_grid.hex;

using vector;

public class CubeHex
{
    public CubeHexVector Position { get; private set; }

    public CubeHex()
    {
        Position = CubeHexVector.Zero;
    }
    
    public CubeHex(int q, int r)
    {
        Position = new CubeHexVector(q, r);
    }
}