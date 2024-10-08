namespace hex_grid.scripts.hex_grid.hex;

using vector;

public class CubeHex
{
    public CubeHexVector Position { get; private set; }
    public float Size { get; private set; }
    public int LibraryIndex { get; private set; }

    public CubeHex(float size, int libraryIndex)
    {
        Position = CubeHexVector.Zero;
        LibraryIndex = libraryIndex;
        Size = size;
    }
    
    public CubeHex(int q, int r, float size, int libraryIndex)
    {
        Position = new CubeHexVector(q, r);
        LibraryIndex = libraryIndex;
        Size = size;
    }
}