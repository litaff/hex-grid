namespace HexGridObject.Providers.Position;

using System;
using System.Collections.Generic;
using Godot;
using HexGridMap;
using HexGridMap.Vector;

public class HexGridPositionProvider : IHexGridPositionProvider
{
    private readonly HeightData heightData;
    private Dictionary<int, IHexStateProvider> hexStateProviders;
    
    public CubeHexVector Position { get; private set; }
    
    /// <summary>
    /// Triggers when position is changed and provides the old position.
    /// </summary>
    public event Action<CubeHexVector>? OnPositionChanged;
    /// <summary>
    /// Triggers when the layer is to be changed and provides the relative layer index.
    /// </summary>
    public event Action<int>? OnLayerChangeRequested;

    public HexGridPositionProvider(CubeHexVector initialPosition, HeightData heightData)
    {
        Position = initialPosition;
        this.heightData = heightData;
        hexStateProviders = new Dictionary<int, IHexStateProvider>();
    }

    public void Enable(HexStateProviders providers)
    {
        hexStateProviders = providers.Providers.ToDictionary();
    }

    public void Disable()
    {
        hexStateProviders.Clear();
    }
    
    public void Translate(CubeHexVector offset)
    {
        var targetPosition = Position + offset;
        
        if (PlaneTranslate(targetPosition)) return;
        
        if (LayerTranslate(targetPosition)) return;
    }

    public bool CanTranslate(CubeHexVector offset)
    {
        if (hexStateProviders.Count == 0) return false;
        
        var targetPosition = Position + offset;
        
        if (CanPlaneTranslate(targetPosition)) return true;
        
        if (CanLayerTranslateTo(targetPosition, -1) || 
            CanLayerTranslateTo(targetPosition, 1)) return true;
        
        return false;
    }

    public bool PlaneTranslate(CubeHexVector targetPosition)
    {
        if (hexStateProviders.Count == 0) return false;
        if (!CanPlaneTranslate(targetPosition)) return false;
        var previousPosition = Position;
        Position = targetPosition;
        OnPositionChanged?.Invoke(previousPosition);
        return true;
    }

    public bool LayerTranslate(CubeHexVector targetPosition)
    {
        if (hexStateProviders.Count == 0) return false;
        return LayerTranslateTo(targetPosition, -1) || 
               LayerTranslateTo(targetPosition, 1);
    }

    public bool LayerTranslateTo(CubeHexVector targetPosition, int relativeLayerIndex)
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

    private bool CanLayerTranslateTo(CubeHexVector targetPosition, int relativeLayerIndex)
    {
        if (!hexStateProviders.TryGetValue(relativeLayerIndex, out var targetHexProvider)) return false;
        
        if (!targetHexProvider.Exists(targetPosition)) return false;
        
        if (!IsStepValid(GetHeightDifference(targetPosition, targetHexProvider))) return false;

        return true;
    }

    private bool CanPlaneTranslate(CubeHexVector targetPosition)
    {
        var isSpaceToStand = true;
        if (hexStateProviders.TryGetValue(1, out var targetProvider))
        {
            isSpaceToStand = hexStateProviders[0].GetHexHeight(targetPosition) < HexGridProperties.LayerHeight ||
                             !targetProvider.Exists(targetPosition);
        }
        
        return hexStateProviders[0].Exists(targetPosition) &&
               IsStepValid(GetHeightDifference(targetPosition, hexStateProviders[0])) &&
               isSpaceToStand;
    }

    private bool IsStepValid(float step)
    {
        return step <= heightData.StepHeight;
    }

    private float GetHeightDifference(CubeHexVector target, IHexStateProvider targetProvider)
    {
        var currentPositionHeight = hexStateProviders[0].GetHexHeight(Position) - heightData.Height;
        return Mathf.Abs(targetProvider.GetHexHeight(target) - currentPositionHeight);
    }
}