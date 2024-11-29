namespace hex_grid.scripts.hex_grid.grid_object;

using Godot;
using managers;
using providers;
using providers.position;
using providers.translation;
using providers.translation.providers;
using vector;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class GridObject : Node3D, ITranslatable
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private float height;
    [Export]
    private float stepHeight;

    private IGridObjectLayerManager layerManager;
    
    public IGridPositionProvider GridPositionProvider { get; protected set; }
    public ITranslationProvider TranslationProvider { get; protected set; }

    public HeightData HeightData => new(height, stepHeight);
    public CubeHexVector GridPosition => GridPositionProvider?.Position ?? AwakePosition;
    private CubeHexVector AwakePosition => new(awakePosition.X, awakePosition.Y);

    public override void _Process(double delta)
    {
        if (TranslationProvider is IUpdateableTranslationProvider updateableTranslationProvider)
        {
            updateableTranslationProvider.Update(delta);
        }
        base._Process(delta);
    }

    public virtual void Enable(IGridObjectLayerManager layerManager, HexStateProviders hexStateProviders)
    {
        this.layerManager = layerManager;
        GridPositionProvider ??= new GridPositionProvider(GridPosition, HeightData);
        GridPositionProvider.Enable(hexStateProviders);
        TranslationProvider ??= new InstantTranslationProvider(this, HeightData);
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
        layerManager.ChangeGridObjectLayer(this, relativeIndex);
    }

    private void OnGridPositionChangedHandler(CubeHexVector oldPosition)
    {
        layerManager.UpdateGridObjectPosition(this, oldPosition);
        // Translation should always happen after the position change.
        TranslationProvider.TranslateTo(GridPositionProvider.Position);
    }
}