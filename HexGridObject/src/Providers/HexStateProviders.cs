namespace HexGridObject.Providers;

using System;
using System.Collections.Generic;

public readonly struct HexStateProviders
{
    public IReadOnlyDictionary<int, IHexStateProvider> Providers { get; }

    public HexStateProviders()
    {
        Providers = new Dictionary<int, IHexStateProvider>();
        throw new ArgumentException("[HexStateProviders] There is no providers.");
    }
    
    public HexStateProviders(Dictionary<int, IHexStateProvider> providers)
    {
        Providers = providers;
        if (providers.ContainsKey(0)) return;
        throw new ArgumentException("[HexStateProviders] There is no provider for the current layer.");
    }
}