namespace HexGridObject.Managers;

using HexGridMap.Vector;

public interface IHexGridObjectLayerManager
{
    public void UpdateGridObjectPosition(HexGridObject hexGridObject, CubeHexVector previousPosition);
    public void ChangeGridObjectLayer(HexGridObject hexGridObject, int relativeLayerIndex);
}