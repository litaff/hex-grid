namespace hex_grid.scripts.hex_grid.hex;

using System.Text.Json.Serialization;
using vector;

[JsonDerivedType(typeof(AccessibleHex), typeDiscriminator: nameof(AccessibleHex))]
public class CubeHex
{
    public CubeHexVector Position { get; private set; }
    public float Size { get; private set; }
    public int LibraryIndex { get; private set; }

    public bool IsOccluder { get; set; }
    
    [JsonIgnore]
    public virtual HexType Type => HexType.Base;
    
    public CubeHex(int q, int r, float size, int libraryIndex)
    {
        Position = new CubeHexVector(q, r);
        LibraryIndex = libraryIndex;
        Size = size;
    }
    
    [JsonConstructor]
    public CubeHex(CubeHexVector position, float size, int libraryIndex, bool isOccluder)
    {
        Position = position;
        Size = size;
        LibraryIndex = libraryIndex;
        IsOccluder = isOccluder;
    }

    public void SetSize(float cellSize)
    {
        if (cellSize <= 0) return;
        Size = cellSize;
    }
}