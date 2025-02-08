namespace HexGridObject.Managers;

using System.Collections.Generic;
using HexGridMap;
using Providers;

public class HexGridObjectManager : IHexGridObjectManager
{
    private readonly Dictionary<int, HexGridObjectLayerManager> layerManagers;

    public IReadOnlyDictionary<int, HexGridObjectLayerManager> LayerManagers => layerManagers;
    
    public HexGridObjectManager(Dictionary<int, IHexProvider> hexProviders)
    {
        layerManagers = new Dictionary<int, HexGridObjectLayerManager>();
        foreach (var hexProvider in hexProviders)
        {
            layerManagers.Add(hexProvider.Key, new HexGridObjectLayerManager(hexProvider.Key, hexProvider.Value, this));
        }
    }
    
    public void AddGridObject(HexGridObject hexGridObject, int layer)
    {
        if (!layerManagers.TryGetValue(layer, out var layerManager)) return;
        layerManager.AddGridObject(hexGridObject);
        hexGridObject.Enable(layerManager, GetProviders(layer, layerManager.Layer));
        hexGridObject.RegisterHandlers();
    }
    
    public void RemoveGridObject(HexGridObject hexGridObject, int layer)
    {
        if (!layerManagers.TryGetValue(layer, out var layerManager)) return;
        hexGridObject.UnregisterHandlers();
        hexGridObject.Disable();
        layerManager.RemoveGridObject(hexGridObject);
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