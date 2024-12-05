namespace addons.hex_grid_editor;

using HexGridMap.Fov;

public interface IHexGridMapEditionProvider : IHexGridMapInitializer, IHexGridMapPropertyHandler, IHexGridMapManager
{
    public IFovProvider FovProvider { get; }
}