namespace HexGridMap;

using System;
using System.Collections.Generic;
using System.Linq;
using Fov;
using Godot;
using Hex;
using Layer;
using Storage;
using Vector;

public class HexGridMap : IFovProvider
{
    private readonly MeshLibrary meshLibrary;
    private readonly IHexMapDataProvider hexMapDataProvider;
    private readonly Dictionary<int, LayerStorage> layers;
    private readonly World3D world;

    public IReadOnlyDictionary<int, LayerStorage> Layers => layers;

    public HexGridMap(MeshLibrary meshLibrary, IHexMapDataProvider hexMapDataProvider,
        World3D world)
    {
        this.meshLibrary = meshLibrary;
        this.world = world;
        this.hexMapDataProvider = hexMapDataProvider;

        layers = new Dictionary<int, LayerStorage>();
        foreach (var data in hexMapDataProvider.GetMaps())
        {
            layers.Add(data.Key, CreateLayer(data.Key, data.Value));
        }
    }
    
    public void AddHex(CubeHex hex, int layerIndex)
    {
        if (layers.TryGetValue(layerIndex, out var layer))
        {
            layer.AddHex(hex);
            return;
        }

        var data = hexMapDataProvider.AddMap(layerIndex);
        layer = CreateLayer(layerIndex, data);
        layers.Add(layerIndex, layer);
        layer.AddHex(hex);
    }
    
    public void RemoveHex(CubeHexVector hexPosition, int layerIndex)
    {
        if (!layers.TryGetValue(layerIndex, out var layer)) return;
        layer.RemoveHex(hexPosition);
        
        if (!layer.IsEmpty()) return;
        layers.Remove(layerIndex);
        hexMapDataProvider.RemoveMap(layerIndex);
    }

    public CubeHex? GetHex(CubeHexVector hexPosition, int layerIndex)
    {
        return layers.TryGetValue(layerIndex, out var layer) ? layer.GetHex(hexPosition) : null;
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions, int[] layerIndexes)
    {
        foreach (var layerIndex in layerIndexes)
        {
            if (!layers.TryGetValue(layerIndex, out var layer)) continue;
            layer.HideChunks(chunkPositions);
        }
    }
    
    public void HideChunks(CubeHexVector[] chunkPositions, Predicate<int> layerPredicate)
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

    public CubeHexVector[] GetVisiblePositions(CubeHexVector origin, int radius, int layerIndex)
    {
        return layers.TryGetValue(layerIndex, out var layer) ? layer.GetVisiblePositions(origin, radius) : [];
    }

    public void Dispose()
    {
        foreach (var layer in layers.Values)
        {
            layer.Dispose();
        }
    }

    private LayerStorage CreateLayer(int layerIndex, IHexMapData data)
    {
        var layerStorage = new LayerStorage(data, layerIndex, meshLibrary, world);
        return layerStorage;
    }
}