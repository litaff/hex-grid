namespace HexGridObject;

using HexGridMap.Vector;
using Managers;
using Providers;
using Providers.Position;
using Providers.Translation;

public class HexGridObject
{
    private IHexGridObjectLayerManager? layerManager;
    
    public IHexGridPositionProvider PositionProvider { get; }
    public ITranslationProvider TranslationProvider { get; }

    public HeightData HeightData { get; }
    public CubeHexVector GridPosition => PositionProvider.Position;

    public HexGridObject(IHexGridPositionProvider positionProvider, ITranslationProvider translationProvider, HeightData heightData)
    {
        PositionProvider = positionProvider;
        TranslationProvider = translationProvider;
        HeightData = heightData;
    }

    public virtual void Enable(IHexGridObjectLayerManager layerManager, HexStateProviders hexStateProviders)
    {
        this.layerManager = layerManager;
        PositionProvider.Enable(hexStateProviders);
        TranslationProvider.Enable(hexStateProviders.Providers[0]);
    }

    public virtual void Disable()
    {
        PositionProvider.Disable();
        TranslationProvider.Disable();
    }

    public virtual void RegisterHandlers()
    {
        PositionProvider.OnPositionChanged += OnPositionChangedHandler;
        PositionProvider.OnLayerChangeRequested += OnLayerChangeRequestedHandler;
    }
    
    public virtual void UnregisterHandlers()
    {
        PositionProvider.OnPositionChanged -= OnPositionChangedHandler;
        PositionProvider.OnLayerChangeRequested -= OnLayerChangeRequestedHandler;
    }
    
    private void OnLayerChangeRequestedHandler(int relativeIndex)
    {
        layerManager?.ChangeGridObjectLayer(this, relativeIndex);
    }

    private void OnPositionChangedHandler(CubeHexVector oldPosition)
    {
        layerManager?.UpdateGridObjectPosition(this, oldPosition);
        // Translation should always happen after the position change.
        TranslationProvider.TranslateTo(PositionProvider.Position);
    }
}