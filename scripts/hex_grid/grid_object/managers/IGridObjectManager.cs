namespace hex_grid.scripts.hex_grid.grid_object.managers;

public interface IGridObjectManager
{
    public void AddGridObject(GridObject gridObject, int layer);
    public void RemoveGridObject(GridObject gridObject, int layer);
}