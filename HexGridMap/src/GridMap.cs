namespace HexGrid.Map;

using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using Renderer;
using Vector;

public class GridMap
{
    private readonly IHexMapDataProvider hexMapDataProvider;
    private readonly Dictionary<int, Layer> layers;
    private readonly IRendererMapFactory rendererMapFactory;

    public IReadOnlyDictionary<int, Layer> Layers => layers;

    public GridMap(IHexMapDataProvider hexMapDataProvider, IRendererMapFactory rendererMapFactory)
    {
        this.rendererMapFactory = rendererMapFactory;
        this.hexMapDataProvider = hexMapDataProvider;

        layers = new Dictionary<int, Layer>();
        foreach (var data in hexMapDataProvider.GetData())
        {
            layers.Add(data.Key, CreateLayer(data.Key, data.Value));
        }
    }
    
    public void AddHex(Hex.Hex hex, int layerIndex)
    {
        if (layers.TryGetValue(layerIndex, out var layer))
        {
            layer.AddHex(hex);
            return;
        }

        var data = hexMapDataProvider.AddData(layerIndex);
        layer = CreateLayer(layerIndex, data);
        layers.Add(layerIndex, layer);
        layer.AddHex(hex);
    }
    
    public void RemoveHex(HexVector hexPosition, int layerIndex)
    {
        if (!layers.TryGetValue(layerIndex, out var layer)) return;
        layer.RemoveHex(hexPosition);
        
        if (!layer.IsEmpty()) return;
        layers.Remove(layerIndex);
        hexMapDataProvider.RemoveData(layerIndex);
    }

    public Hex.Hex? GetHex(HexVector hexPosition, int layerIndex)
    {
        return layers.TryGetValue(layerIndex, out var layer) ? layer.GetHex(hexPosition) : null;
    }
    
    public void HideLayers(int[] layerIndexes)
    {
        foreach (var layerIndex in layerIndexes)
        {
            if (!layers.TryGetValue(layerIndex, out var layer)) continue;
            layer.Hide();
        }
        
        foreach (var layer in layers.Where(layer => !layerIndexes.Contains(layer.Key)))
        {
            layer.Value.Display();
        }
    }
    
    public void HideLayers(Predicate<int> predicate)
    {
        foreach (var layer in layers.Where(layer => predicate(layer.Key)))
        {
            layer.Value.Hide();
        }
        
        foreach (var layer in layers.Where(layer => !predicate(layer.Key)))
        {
            layer.Value.Display();
        }
    }

    public void Dispose()
    {
        foreach (var layer in layers.Values)
        {
            layer.Dispose();
        }
    }

    private Layer CreateLayer(int layerIndex, IHexMapData mapData)
    {
        var rendererMap = rendererMapFactory.New(layerIndex);
        mapData.Deserialize();
        rendererMap.AddTo(mapData.Map.Values.ToArray(), HexVector.Zero);
        var layerStorage = new Layer(mapData, rendererMap);
        return layerStorage;
    }
}