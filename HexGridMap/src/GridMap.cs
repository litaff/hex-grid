namespace HexGrid.Map;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hex;
using Renderer;
using Vector;

public class GridMap
{
    public HexMap HexMap { get; }
    public IRendererMap RendererMap { get; }

    public GridMap(IHexMapDataProvider hexMapDataProvider, IRendererMap rendererMap)
    {
        var data = hexMapDataProvider.GetData();
        HexMap = new HexMap(data);
        RendererMap = rendererMap;
        RendererMap.AddTo(HexMap.GetMap(), HexVector.Zero);
    }
    
    public void AddHex(Hex.Hex hex)
    {
        HexMap.Add(hex);
        RendererMap.AddHex(hex);
    }
    
    public void RemoveHex(HexVector position)
    {
        HexMap.Remove(position);
        RendererMap.RemoveHex(position);
    }

    public Hex.Hex? GetHex(HexVector hexPosition)
    {
        return HexMap.Get(hexPosition);
    }
    
    public void Occlude(Predicate<HexVector>? hexPositionPredicate)
    {
        GD.Print("Implement layer hiding");
    }

    public void Dispose()
    {
        RendererMap.Dispose();
    }
}