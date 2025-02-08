namespace HexGridObject;

using HexGridMap.Vector;
using Managers;
using Providers;
using Providers.Position;
using Providers.Translation;

public class HexGridObject
{
    private IHexGridObjectLayerManager? layerManager;
    
    public IHexGridPositionProvider HexGridPositionProvider { get; }
    public ITranslationProvider TranslationProvider { get; }

    public HeightData HeightData { get; }
    public CubeHexVector GridPosition => HexGridPositionProvider.Position;

    public HexGridObject(IHexGridPositionProvider hexGridPositionProvider, ITranslationProvider translationProvider, HeightData heightData)
    {
        HexGridPositionProvider = hexGridPositionProvider;
        TranslationProvider = translationProvider;
        HeightData = heightData;
    }

    public virtual void Enable(IHexGridObjectLayerManager layerManager, HexStateProviders hexStateProviders)
    {
        this.layerManager = layerManager;
        HexGridPositionProvider.Enable(hexStateProviders);
        TranslationProvider.Enable(hexStateProviders.Providers[0]);
    }

    public virtual void Disable()
    {
        HexGridPositionProvider.Disable();
        TranslationProvider.Disable();
    }

    public virtual void RegisterHandlers()
    {
        HexGridPositionProvider.OnPositionChanged += OnHexGridPositionChangedHandler;
        HexGridPositionProvider.OnLayerChangeRequested += OnLayerChangeRequestedHandler;
    }
    
    public virtual void UnregisterHandlers()
    {
        HexGridPositionProvider.OnPositionChanged -= OnHexGridPositionChangedHandler;
        HexGridPositionProvider.OnLayerChangeRequested -= OnLayerChangeRequestedHandler;
    }
    
    private void OnLayerChangeRequestedHandler(int relativeIndex)
    {
        layerManager?.ChangeGridObjectLayer(this, relativeIndex);
    }

    private void OnHexGridPositionChangedHandler(CubeHexVector oldPosition)
    {
        layerManager?.UpdateGridObjectPosition(this, oldPosition);
        // Translation should always happen after the position change.
        TranslationProvider.TranslateTo(HexGridPositionProvider.Position);
    }
}