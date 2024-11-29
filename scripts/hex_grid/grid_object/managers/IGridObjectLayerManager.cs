namespace hex_grid.scripts.hex_grid.grid_object.managers;

using vector;

public interface IGridObjectLayerManager
{
    public void UpdateGridObjectPosition(GridObject gridObject, CubeHexVector previousPosition);
    public void ChangeGridObjectLayer(GridObject gridObject, int relativeLayerIndex);
}