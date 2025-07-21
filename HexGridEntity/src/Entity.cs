namespace HexGrid.Entity;

using Handlers.Position;
using Handlers.Rotation;
using Manager;
using Map.Vector;
using Providers;
using Providers.Block;
using Providers.Position;
using Providers.Rotation;

public class Entity
{
    private IEntityManager? entityManager;
    
    public IPositionProvider PositionProvider { get; }
    public IPositionHandler PositionHandler { get; }
    public IBlockProvider BlockProvider { get; }
    public IRotationProvider RotationProvider { get; }
    public IRotationHandler RotationHandler { get; }

    public bool Enabled { get; private set; }
    public HeightData HeightData { get; }
    public HexVector GridPosition => PositionProvider.Position;

    public Entity(IPositionProvider positionProvider, IRotationProvider rotationProvider, 
        IPositionHandler positionHandler, IRotationHandler rotationHandler, IBlockProvider blockProvider, HeightData heightData)
    {
        PositionProvider = positionProvider;
        RotationProvider = rotationProvider;
        PositionHandler = positionHandler;
        RotationHandler = rotationHandler;
        BlockProvider = blockProvider;
        HeightData = heightData;
    }

    public virtual void Enable(IEntityManager entityManager, IHexStateProvider hexStateProvider)
    {
        Enabled = true;
        this.entityManager = entityManager;
        PositionHandler.Enable(hexStateProvider);
        PositionProvider.Enable(hexStateProvider);
        RotationProvider.Enable();
    }

    public virtual void Disable()
    {
        Enabled = false;
        RotationProvider.Disable();
        PositionProvider.Disable();
        PositionHandler.Disable();
    }

    public virtual void RegisterHandlers()
    {
        PositionProvider.OnPositionChanged += OnPositionChangedHandler;
        RotationProvider.OnRotationChanged += OnRotationChangedHandler;
    }

    public virtual void UnregisterHandlers()
    {
        PositionProvider.OnPositionChanged -= OnPositionChangedHandler;
        RotationProvider.OnRotationChanged -= OnRotationChangedHandler;
    }

    private void OnPositionChangedHandler(HexVector oldPosition)
    {
        entityManager?.MoveFrom(this, oldPosition);
        // Translation should always happen after the position change.
        PositionHandler.TranslateTo(PositionProvider.Position);
    }

    private void OnRotationChangedHandler()
    {
        RotationHandler.RotateTowards(RotationProvider.Forward);
    }
}