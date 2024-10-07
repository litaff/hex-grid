namespace hex_grid.scripts.hex_grid.chunk;

using vector;

public struct CubeChunk
{
    public CubeHexVector Position { get; private set; }
    
    public CubeChunk(CubeHexVector position)
    {
        Position = position;
    }
}