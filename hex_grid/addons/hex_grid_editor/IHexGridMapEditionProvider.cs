namespace addons.hex_grid_editor;

using hex_grid_map.fov;

public interface IHexGridMapEditionProvider : IHexGridMapInitializer, IHexGridMapPropertyHandler, IHexGridMapManager
{
    public IFovProvider FovProvider { get; }
}