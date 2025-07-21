namespace HexGrid.Map.Vector;

public struct HexFracVector
{
    public float Q { get; private set; }
    public float R { get; private set; }
    public float S { get; private set; }
    public float H { get; private set; }
    
    public HexFracVector(float q, float r, float h = 0)
    {
        Q = q;
        R = r;
        S = -q - r;
        H = h;
    }
    
    public override string ToString()
    {
        return $"({Q}, {R}, {S}, {H})";
    }
}