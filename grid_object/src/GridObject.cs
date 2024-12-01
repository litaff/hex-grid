namespace grid_object;

using hex_grid_map.vector;
using managers;
using providers;
using providers.position;
using providers.translation.providers;

public class GridObject
{
    private IGridObjectLayerManager? layerManager;
    
    public IGridPositionProvider GridPositionProvider { get; }
    public ITranslationProvider TranslationProvider { get; }

    public HeightData HeightData { get; }
    public CubeHexVector GridPosition => GridPositionProvider.Position;

    public GridObject(IGridPositionProvider gridPositionProvider, ITranslationProvider translationProvider, HeightData heightData)
    {
        GridPositionProvider = gridPositionProvider;
        TranslationProvider = translationProvider;
        HeightData = heightData;
    }

    public virtual void Enable(IGridObjectLayerManager layerManager, HexStateProviders hexStateProviders)
    {
        this.layerManager = layerManager;
        GridPositionProvider.Enable(hexStateProviders);
        TranslationProvider.Enable(hexStateProviders.Providers[0]);
    }

    public virtual void Disable()
    {
        GridPositionProvider.Disable();
        TranslationProvider.Disable();
    }

    public virtual void RegisterHandlers()
    {
        GridPositionProvider.OnPositionChanged += OnGridPositionChangedHandler;
        GridPositionProvider.OnLayerChangeRequested += OnLayerChangeRequestedHandler;
    }
    
    public virtual void UnregisterHandlers()
    {
        GridPositionProvider.OnPositionChanged -= OnGridPositionChangedHandler;
        GridPositionProvider.OnLayerChangeRequested -= OnLayerChangeRequestedHandler;
    }
    
    private void OnLayerChangeRequestedHandler(int relativeIndex)
    {
        layerManager?.ChangeGridObjectLayer(this, relativeIndex);
    }

    private void OnGridPositionChangedHandler(CubeHexVector oldPosition)
    {
        layerManager?.UpdateGridObjectPosition(this, oldPosition);
        // Translation should always happen after the position change.
        TranslationProvider.TranslateTo(GridPositionProvider.Position);
    }
}