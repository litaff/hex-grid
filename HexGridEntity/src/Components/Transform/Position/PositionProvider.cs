namespace HexGrid.Entity.Components.Transform.Position;

using Blocker;
using Interfaces;
using Managers;
using Map.Vector;

public class PositionProvider
{
    private readonly int stepRange;
    private readonly IHexManager hexManager;
    private readonly IBlockerManager blockerManager;

    private HexVector targetPosition;
    private IPositionHandler? handler;

    public bool Enabled { get; private set; }
    public HexVector Current { get; private set; }
    public HexVector Previous { get; private set; }
    public bool IsMoving => handler is not null && !handler.MoveComplete;
    
    public event Action? OnPositionChanged;

    public PositionProvider(IHexManager hexManager, IBlockerManager blockerManager, HexVector initialPosition, int stepRange)
    {
        this.hexManager = hexManager;
        Current = initialPosition;
        Previous = initialPosition;
        this.stepRange = stepRange;
        this.blockerManager = blockerManager;
        Enabled = false;
    }

    public void Enable()
    {
        handler?.Enable();
        Enabled = true;
        OnPositionChanged?.Invoke();
    }

    public void Disable()
    {
        handler?.Disable();
        Enabled = false;
    }

    public void SetHandler(IPositionHandler handler)
    {
        this.handler = handler;
    }
    
    public void Translate(HexVector offset)
    {
        if (!Enabled) return;
        
        if (!CanTranslate(offset, out var target)) return;

        targetPosition = target;

        if (handler is null)
        {
            OnPositionReadyHandler();
            return;
        }
        
        handler.OnPositionReady += OnPositionReadyHandler;
        handler.MoveTo(targetPosition);
    }

    public bool CanTranslate(HexVector offset, out HexVector closest)
    {
        closest = Current;
        
        var target = Current + offset;

        if (!hexManager.TryGetClosest(target, stepRange, out closest)) return false;

        return CanFit(closest) && 
               !IsBlocked(closest);
    }

    public void Update(double deltaTime)
    {
        if (!Enabled) return;
        handler?.Update(deltaTime);
    }

    private void OnPositionReadyHandler()
    {
        if (handler is not null)
        {
            handler.OnPositionReady -= OnPositionReadyHandler;
        }
        Previous = Current;
        Current = targetPosition;
        OnPositionChanged?.Invoke();
    }

    private bool CanFit(HexVector target)
    {
        var isSpaceToStand = true; // TODO: This

        if (!isSpaceToStand) return false;

        return true;
    }

    private bool IsBlocked(HexVector target)
    {
        return blockerManager.IsBlocked(target, Current.DirectionTo(target).Normalized()) || 
               blockerManager.IsBlocked(Current, target.DirectionTo(Current).Normalized());
    }
}