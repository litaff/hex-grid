namespace addons.hex_grid_map_editor;

using HexGrid.Map.Vector;

public interface IHexGridMapEditionProvider : IHexGridMapInitializer, IHexGridMapPropertyHandler, IHexGridMapManager
{
    public HexVectorConverter? Converter { get; }
}