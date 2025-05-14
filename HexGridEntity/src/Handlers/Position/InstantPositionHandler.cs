namespace HexGrid.Entity.Handlers.Position;

using Godot;
using Providers;
using Map.Vector;

public class InstantPositionHandler : IPositionHandler
{
    private readonly ITranslatable translatable;
    private readonly HeightData heightData;
    
    private IHexStateProvider? hexStateProvider;

    public InstantPositionHandler(ITranslatable translatable, HeightData heightData)
    {
        this.translatable = translatable;
        this.heightData = heightData;
    }

    public void TranslateTo(HexVector position)
    {
        if (hexStateProvider == null) return;
        
        var offset = position.ToWorldPosition() + GetHexHeightVector(position) - translatable.Position;
        translatable.Translate(offset);
    }

    public void Enable(IHexStateProvider provider)
    {
        hexStateProvider = provider;
    }

    public void Disable()
    {
        hexStateProvider = null;
    }

    private Vector3 GetHexHeightVector(HexVector position)
    {
        if (hexStateProvider == null) return Vector3.Zero;
        return (hexStateProvider.GetHexHeight(position) - heightData.Height) * Vector3.Up;
    }
}