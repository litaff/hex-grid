namespace hex_grid.scripts;

public struct CubeHexChunk
{
    public CubeHexVector Position { get; private set; }
    
    public CubeHexChunk(CubeHexVector position)
    {
        Position = position;
    }
}