namespace addons.hex_grid_map_editor;

using System;
using System.Collections.Generic;
using HexGrid.Map.Fov;
using HexGrid.Map.Hex;
using HexGrid.Map.Vector;

public interface IHexGridMapManager
{
    public Dictionary<int, IFovProvider> FovProviders { get; }
    public Hex? GetHex(HexVector hexPosition, int layerIndex);
    public void AddHex(Hex hex, int layerIndex);
    public void RemoveHex(HexVector hexPosition, int layerIndex);
    public void ClearMap(int index);
    public void ClearMaps();
    public void HideLayers(int[] layerIndexes);
    public void HideLayers(Predicate<int> predicate);
}