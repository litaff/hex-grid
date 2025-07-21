namespace addons.hex_grid_map_editor;

using System;
using HexGrid.Map.Fov;
using HexGrid.Map.Hex;
using HexGrid.Map.Vector;

public interface IHexGridMapManager
{
    public IFovProvider FovProvider { get; }
    public Hex? GetHex(HexVector hexPosition);
    public void AddHex(Hex hex);
    public void RemoveHex(HexVector hexPosition);
    public void ClearMap();
    public void ClearMaps();
    public void Occlude(Predicate<HexVector>? hexPositionPredicate);
}