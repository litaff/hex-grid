namespace hex_grid.scripts;

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