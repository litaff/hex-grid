namespace HexGridMap.Vector;

public struct CubeHexFracVector
{
    public float Q { get; private set; }
    public float R { get; private set; }
    public float S { get; private set; }
    
    public CubeHexFracVector(float q, float r)
    {
        Q = q;
        R = r;
        S = -q - r;
    }
    
    public override string ToString()
    {
        return $"({Q}, {R}, {S})";
    }
}