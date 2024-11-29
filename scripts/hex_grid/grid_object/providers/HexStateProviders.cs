namespace hex_grid.scripts.hex_grid.grid_object.providers;

using System;
using System.Collections.Generic;

public readonly struct HexStateProviders
{
    public IReadOnlyDictionary<int, IHexStateProvider> Providers { get; }
    
    public HexStateProviders(Dictionary<int, IHexStateProvider> providers)
    {
        Providers = providers;
        if (providers.ContainsKey(0)) return;
        throw new ArgumentException("[HexStateProviders] There is no provider for the current layer.");
    }
}