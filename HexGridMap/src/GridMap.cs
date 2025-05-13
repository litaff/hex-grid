namespace HexGrid.Map;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hex;
using Vector;

public class GridMap
{
    private readonly MeshLibrary meshLibrary;
    private readonly IHexMapDataProvider hexMapDataProvider;
    private readonly Dictionary<int, Layer> layers;
    private readonly World3D world;

    public IReadOnlyDictionary<int, Layer> Layers => layers;

    public GridMap(MeshLibrary meshLibrary, IHexMapDataProvider hexMapDataProvider,
        World3D world)
    {
        this.meshLibrary = meshLibrary;
        this.world = world;
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
    
    public void HideChunks(HexVector[] chunkPositions, int[] layerIndexes)
    {
        foreach (var layerIndex in layerIndexes)
        {
            if (!layers.TryGetValue(layerIndex, out var layer)) continue;
            layer.HideChunks(chunkPositions);
        }
    }
    
    public void HideChunks(HexVector[] chunkPositions, Predicate<int> layerPredicate)
    {
        foreach (var layer in layers.Where(layer => layerPredicate(layer.Key)))
        {
            layer.Value.HideChunks(chunkPositions);
        }
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
        var layerStorage = new Layer(mapData, layerIndex, meshLibrary, world);
        return layerStorage;
    }
}