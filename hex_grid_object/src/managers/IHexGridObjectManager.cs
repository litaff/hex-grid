namespace hex_grid_object.managers;

public interface IHexGridObjectManager
{
    public void AddGridObject(HexGridObject hexGridObject, int layer);
    public void RemoveGridObject(HexGridObject hexGridObject, int layer);
}