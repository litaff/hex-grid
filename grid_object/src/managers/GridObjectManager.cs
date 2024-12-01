namespace grid_object.managers;

using System.Collections.Generic;
using hex_grid_map.interfaces;
using providers;

public class GridObjectManager : IGridObjectManager
{
    private readonly Dictionary<int, GridObjectLayerManager> layerManagers;

    public GridObjectManager(Dictionary<int, IHexProvider> hexProviders)
    {
        layerManagers = new Dictionary<int, GridObjectLayerManager>();
        foreach (var hexProvider in hexProviders)
        {
            layerManagers.Add(hexProvider.Key, new GridObjectLayerManager(hexProvider.Key, hexProvider.Value, this));
        }
    }
    
    public void AddGridObject(GridObject gridObject, int layer)
    {
        if (!layerManagers.TryGetValue(layer, out var gridObjectManager)) return;
        gridObjectManager.AddGridObject(gridObject);
        gridObject.Enable(gridObjectManager, GetProviders(layer, gridObjectManager.Layer));
        gridObject.RegisterHandlers();
    }
    
    public void RemoveGridObject(GridObject gridObject, int layer)
    {
        if (!layerManagers.TryGetValue(layer, out var gridObjectLayer)) return;
        gridObject.UnregisterHandlers();
        gridObject.Disable();
        gridObjectLayer.RemoveGridObject(gridObject);
    }

    private HexStateProviders GetProviders(int layer, int relativeLayer)
    {
        var providers = new Dictionary<int, IHexStateProvider>();
        
        for (var i = layer - 1; i <= layer + 1; i++)
        {
            if (!layerManagers.TryGetValue(i, out var manager)) continue;
            providers.Add(i - relativeLayer, manager);
        }

        return new HexStateProviders(providers);
    }
}