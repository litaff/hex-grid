namespace hex_grid.scripts.hex_grid;

using vector;

public struct CubeHexChunk
{
    public CubeHexVector Position { get; private set; }
    
    public CubeHexChunk(CubeHexVector position)
    {
        Position = position;
    }
}