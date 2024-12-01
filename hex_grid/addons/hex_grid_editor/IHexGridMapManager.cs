namespace addons.hex_grid_editor;

using System;
using hex_grid_map.hex;
using hex_grid_map.vector;

public interface IHexGridMapManager
{
    public CubeHex? GetHex(CubeHexVector hexPosition, int layerIndex);
    public void AddHex(CubeHex hex, int layerIndex);
    public void RemoveHex(CubeHexVector hexPosition, int layerIndex);
    public void ClearMap(int index);
    public void ClearMaps();
    public void HideLayers(int[] layerIndexes);
    public void HideLayers(Predicate<int> predicate);
}