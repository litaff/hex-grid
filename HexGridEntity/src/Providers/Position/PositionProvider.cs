namespace HexGrid.Entity.Providers.Position;

using System;
using System.Collections.Generic;
using Godot;
using Map;
using Map.Vector;

public class PositionProvider : IPositionProvider
{
    private readonly HeightData heightData;
    private Dictionary<int, IHexStateProvider> hexStateProviders;
    
    public HexVector Position { get; private set; }
    
    /// <summary>
    /// Triggers when position is changed and provides the old position.
    /// </summary>
    public event Action<HexVector>? OnPositionChanged;
    /// <summary>
    /// Triggers when the layer is to be changed and provides the relative layer index.
    /// </summary>
    public event Action<int>? OnLayerChangeRequested;

    public PositionProvider(HexVector initialPosition, HeightData heightData)
    {
        Position = initialPosition;
        this.heightData = heightData;
        hexStateProviders = new Dictionary<int, IHexStateProvider>();
    }

    public void Enable(HexStateProviders providers)
    {
        hexStateProviders = providers.Providers.ToDictionary();
        OnPositionChanged?.Invoke(Position);
    }

    public void Disable()
    {
        hexStateProviders.Clear();
    }
    
    public void Translate(HexVector offset)
    {
        var targetPosition = Position + offset;
        
        if (PlaneTranslate(targetPosition)) return;
        
        if (LayerTranslate(targetPosition)) return;
    }

    public bool CanTranslate(HexVector offset)
    {
        if (hexStateProviders.Count == 0) return false;
        
        var targetPosition = Position + offset;
        
        if (CanPlaneTranslate(targetPosition)) return true;
        
        if (CanLayerTranslateTo(targetPosition, -1) || 
            CanLayerTranslateTo(targetPosition, 1)) return true;
        
        return false;
    }

    public bool PlaneTranslate(HexVector targetPosition)
    {
        if (hexStateProviders.Count == 0) return false;
        if (!CanPlaneTranslate(targetPosition)) return false;
        var previousPosition = Position;
        Position = targetPosition;
        OnPositionChanged?.Invoke(previousPosition);
        return true;
    }

    public bool LayerTranslate(HexVector targetPosition)
    {
        if (hexStateProviders.Count == 0) return false;
        return LayerTranslateTo(targetPosition, -1) || 
               LayerTranslateTo(targetPosition, 1);
    }

    public bool LayerTranslateTo(HexVector targetPosition, int relativeLayerIndex)
    {
        if (!CanLayerTranslateTo(targetPosition, relativeLayerIndex)) return false;

        OnLayerChangeRequested?.Invoke(relativeLayerIndex);
        var previousPosition = Position;
        Position = targetPosition;
        // This will change the stack of the object. Remove does nothing as it was removed on layer change.
        // Only add will have an effect.
        OnPositionChanged?.Invoke(previousPosition);

        return true;
    }

    private bool CanLayerTranslateTo(HexVector targetPosition, int relativeLayerIndex)
    {
        if (!hexStateProviders.TryGetValue(relativeLayerIndex, out var targetHexProvider)) return false;
        
        if (!targetHexProvider.Exists(targetPosition)) return false;
        
        if (!IsStepValid(GetHeightDifference(targetPosition, targetHexProvider))) return false;

        if (targetHexProvider.IsBlocked(targetPosition, Position.DirectionTo(targetPosition).Normalized())) return false;
        
        if (hexStateProviders[0].IsBlocked(Position, targetPosition.DirectionTo(Position).Normalized())) return false;
        
        return true;
    }

    private bool CanPlaneTranslate(HexVector targetPosition)
    {
        var isSpaceToStand = true;
        if (hexStateProviders.TryGetValue(1, out var targetProvider))
        {
            isSpaceToStand = hexStateProviders[0].GetHexHeight(targetPosition) < Properties.LayerHeight ||
                             !targetProvider.Exists(targetPosition);
        }

        if (!isSpaceToStand) return false;
        
        if (!hexStateProviders[0].Exists(targetPosition)) return false;

        if (!IsStepValid(GetHeightDifference(targetPosition, hexStateProviders[0]))) return false;

        if (hexStateProviders[0].IsBlocked(targetPosition, Position.DirectionTo(targetPosition).Normalized()))
            return false;
        
        if (hexStateProviders[0].IsBlocked(Position, targetPosition.DirectionTo(Position).Normalized()))
            return false;

        return true;
    }

    private bool IsStepValid(float step)
    {
        return step <= heightData.StepHeight;
    }

    private float GetHeightDifference(HexVector target, IHexStateProvider targetProvider)
    {
        var currentPositionHeight = hexStateProviders[0].GetHexHeight(Position) - heightData.Height;
        return Mathf.Abs(targetProvider.GetHexHeight(target) - currentPositionHeight);
    }
}