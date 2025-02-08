namespace HexGridMap;

using System;
using System.Collections.Generic;
using System.Linq;
using Fov;
using Godot;
using Hex;
using Vector;

public class HexGridMap : IFovProvider
{
    private readonly MeshLibrary meshLibrary;
    private readonly IHexDataProvider hexDataProvider;
    private readonly Dictionary<int, HexGridLayer> layers;
    private readonly World3D world;

    public IReadOnlyDictionary<int, HexGridLayer> Layers => layers;

    public HexGridMap(MeshLibrary meshLibrary, IHexDataProvider hexDataProvider,
        World3D world)
    {
        this.meshLibrary = meshLibrary;
        this.world = world;
        this.hexDataProvider = hexDataProvider;

        layers = new Dictionary<int, HexGridLayer>();
        foreach (var data in hexDataProvider.GetData())
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

        var data = hexDataProvider.AddData(layerIndex);
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
        hexDataProvider.RemoveData(layerIndex);
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

    private HexGridLayer CreateLayer(int layerIndex, IHexData data)
    {
        var layerStorage = new HexGridLayer(data, layerIndex, meshLibrary, world);
        return layerStorage;
    }
}