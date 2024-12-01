namespace hex_grid_object.managers;

using hex_grid_map.vector;

public interface IHexGridObjectLayerManager
{
    public void UpdateGridObjectPosition(HexGridObject hexGridObject, CubeHexVector previousPosition);
    public void ChangeGridObjectLayer(HexGridObject hexGridObject, int relativeLayerIndex);
}