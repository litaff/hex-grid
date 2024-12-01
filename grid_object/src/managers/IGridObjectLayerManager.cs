namespace grid_object.managers;

using hex_grid_map.vector;

public interface IGridObjectLayerManager
{
    public void UpdateGridObjectPosition(GridObject gridObject, CubeHexVector previousPosition);
    public void ChangeGridObjectLayer(GridObject gridObject, int relativeLayerIndex);
}