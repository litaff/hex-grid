namespace HexGrid.Entity.Providers.Position;

using System;
using Map.Vector;

public class PositionProvider : IPositionProvider
{
    private readonly HeightData heightData;
    private IHexStateProvider? hexStateProvider;
    
    public HexVector Position { get; private set; }
    
    /// <summary>
    /// Triggers when position is changed and provides the old position.
    /// </summary>
    public event Action<HexVector>? OnPositionChanged;

    public PositionProvider(HexVector initialPosition, HeightData heightData)
    {
        Position = initialPosition;
        this.heightData = heightData;
    }

    public void Enable(IHexStateProvider hexStateProvider)
    {
        this.hexStateProvider = hexStateProvider;
        OnPositionChanged?.Invoke(Position);
    }

    public void Disable()
    {
        hexStateProvider = null;
    }
    
    public void Translate(HexVector offset)
    {
        if (!CanTranslate(offset, out var target)) return;
        
        var previousPosition = Position;
        Position = target;
        OnPositionChanged?.Invoke(previousPosition);
    }

    public bool CanTranslate(HexVector offset, out HexVector closest)
    {
        closest = Position;
        
        if (hexStateProvider is null) return false;
        
        var targetPosition = Position + offset;

        if (!hexStateProvider.TryGetClosest(targetPosition, heightData.StepRange, out closest)) return false;

        return IsAvailable(closest);
    }

    private bool IsAvailable(HexVector targetPosition)
    {
        var isSpaceToStand = true; // TODO: This

        if (!isSpaceToStand) return false;

        if (IsBlocked(targetPosition)) return false;

        return true;
    }

    private bool IsBlocked(HexVector targetPosition)
    {
        if (hexStateProvider is null) return true;
        
        if (hexStateProvider.IsBlocked(targetPosition, Position.DirectionTo(targetPosition).Normalized()))
            return true;
        
        if (hexStateProvider.IsBlocked(Position, targetPosition.DirectionTo(Position).Normalized()))
            return true;

        return false;
    }
}