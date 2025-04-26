namespace HexGridObject.Providers;

using HexGridMap.Vector;

public interface IHexStateProvider
{
    public float GetHexHeight(CubeHexVector position, List<HexGridObject>? exclude = null);
    public bool Contains<T>(CubeHexVector position);
    public bool Exists(CubeHexVector position);
}