namespace HexGrid.Entity.Managers;

using System.Collections.Generic;
using Map.Hex;
using Providers;

public class EntityManager : IEntityManager
{
    private readonly Dictionary<int, EntityLayerManager> layerManagers;

    public IReadOnlyDictionary<int, EntityLayerManager> LayerManagers => layerManagers;
    
    public EntityManager(Dictionary<int, IHexProvider> hexProviders)
    {
        layerManagers = new Dictionary<int, EntityLayerManager>();
        foreach (var hexProvider in hexProviders)
        {
            layerManagers.Add(hexProvider.Key, new EntityLayerManager(hexProvider.Key, hexProvider.Value, this));
        }
    }
    
    public void Add(Entity entity, int layer)
    {
        if (!layerManagers.TryGetValue(layer, out var layerManager)) return;
        layerManager.Add(entity);
        entity.Enable(layerManager, GetProviders(layer, layerManager.Layer));
        entity.RegisterHandlers();
    }
    
    public void Remove(Entity entity, int layer)
    {
        if (!layerManagers.TryGetValue(layer, out var layerManager)) return;
        entity.UnregisterHandlers();
        entity.Disable();
        layerManager.Remove(entity);
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