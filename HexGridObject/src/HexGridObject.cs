namespace HexGridObject;

using Handlers.Rotation;
using Handlers.Translation;
using HexGridMap.Vector;
using Managers;
using Providers;
using Providers.Position;
using Providers.Rotation;

public class HexGridObject
{
    private IHexGridObjectLayerManager? layerManager;
    
    public IPositionProvider PositionProvider { get; }
    public IRotationProvider RotationProvider { get; }
    public ITranslationHandler TranslationHandler { get; }
    public IRotationHandler RotationHandler { get; }

    public HeightData HeightData { get; }
    public CubeHexVector GridPosition => PositionProvider.Position;

    public HexGridObject(IPositionProvider positionProvider, IRotationProvider rotationProvider, 
        ITranslationHandler translationHandler, IRotationHandler rotationHandler, HeightData heightData)
    {
        PositionProvider = positionProvider;
        RotationProvider = rotationProvider;
        TranslationHandler = translationHandler;
        RotationHandler = rotationHandler;
        HeightData = heightData;
    }

    public virtual void Enable(IHexGridObjectLayerManager layerManager, HexStateProviders hexStateProviders)
    {
        this.layerManager = layerManager;
        PositionProvider.Enable(hexStateProviders);
        TranslationHandler.Enable(hexStateProviders.Providers[0]);
    }

    public virtual void Disable()
    {
        PositionProvider.Disable();
        TranslationHandler.Disable();
    }

    public virtual void RegisterHandlers()
    {
        PositionProvider.OnPositionChanged += OnPositionChangedHandler;
        RotationProvider.OnRotationChanged += OnRotationChangedHandler;
        PositionProvider.OnLayerChangeRequested += OnLayerChangeRequestedHandler;
    }

    public virtual void UnregisterHandlers()
    {
        PositionProvider.OnPositionChanged -= OnPositionChangedHandler;
        RotationProvider.OnRotationChanged -= OnRotationChangedHandler;
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
        TranslationHandler.TranslateTo(PositionProvider.Position);
    }

    private void OnRotationChangedHandler()
    {
        RotationHandler.RotateTowards(RotationProvider.Forward);
    }
}