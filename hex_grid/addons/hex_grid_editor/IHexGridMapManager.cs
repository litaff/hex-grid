namespace addons.hex_grid_editor;

using System;
using System.Collections.Generic;
using HexGridMap;
using HexGridMap.Fov;
using HexGridMap.Hex;
using HexGridMap.Vector;

public interface IHexGridMapManager
{
    public Dictionary<int, IFovProvider> FovProviders { get; }
    public CubeHex? GetHex(CubeHexVector hexPosition, int layerIndex);
    public void AddHex(CubeHex hex, int layerIndex);
    public void RemoveHex(CubeHexVector hexPosition, int layerIndex);
    public void ClearMap(int index);
    public void ClearMaps();
    public void HideLayers(int[] layerIndexes);
    public void HideLayers(Predicate<int> predicate);
}