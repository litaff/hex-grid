namespace HexGridObject.Providers.Position;

using System;
using System.Collections.Generic;
using Godot;
using HexGridMap.Hex;
using HexGridMap.Vector;

public class HexGridPositionProvider : IHexGridPositionProvider
{
    private readonly HeightData heightData;
    private Dictionary<int, IHexStateProvider> hexStateProviders;
    
    public CubeHexVector Position { get; private set; }
    private float CurrentPositionHeight => hexStateProviders[0].GetHexHeight(Position) - heightData.Height;
    
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
        if (hexStateProviders.Count == 0) return;
        
        // If object is on elevated hex this mean that it can go up or down a layer, so we try to do that first.
        if (hexStateProviders[0].HexIs<ElevatedHex>(Position) && TryElevatedTranslate(offset)) return;
        
        if (!hexStateProviders[0].HexIs<AccessibleHex>(Position + offset)) return;
        AccessibleTranslate(offset);
    }

    private void AccessibleTranslate(CubeHexVector offset)
    {
        if (!IsStepValid(GetHeightDifference(Position + offset))) return;
        Position += offset;
        OnPositionChanged?.Invoke(Position - offset);
    }

    private bool IsStepValid(float step)
    {
        return step <= heightData.StepHeight;
    }

    private float GetHeightDifference(CubeHexVector target)
    {
        return Mathf.Abs(hexStateProviders[0].GetHexHeight(target) - CurrentPositionHeight);
    }

    /// <summary>
    /// Returns false if layer could not be changed.
    /// </summary>
    private bool TryElevatedTranslate(CubeHexVector offset)
    {
        // Prioritize staying on the same layer if possible.
        var targetPosition = Position + offset;
        if (hexStateProviders[0].HexIs<ElevatedHex>(targetPosition) && IsStepValid(GetHeightDifference(targetPosition))) return false;

        // Prioritize going down.
        return TryElevateTo(offset, -1) || TryElevateTo(offset, 1);
    }
    
    /// <summary>
    /// Returns false if this couldn't go to a relative layer.
    /// </summary>
    private bool TryElevateTo(CubeHexVector offset, int relativeLayerIndex)
    {
        if (!hexStateProviders.TryGetValue(relativeLayerIndex, out var targetHexProvider)) return false;
        
        var targetPosition = Position + offset;
        
        if (!targetHexProvider.HexIs<ElevatedHex>(targetPosition)) return false;
        
        var targetHeight = targetHexProvider.GetHexHeight(targetPosition);
        if (!IsStepValid(Mathf.Abs(targetHeight - CurrentPositionHeight))) return false;

        // This will move the object to another layer, potentially on an invalid hex.
        // It doesn't matter as the checks have already been done and the translation will take place to a valid hex.
        OnLayerChangeRequested?.Invoke(relativeLayerIndex);
        Position += offset;
        // This will change the stack of the object. Remove does nothing as it was removed on layer change.
        // Only add will have an effect.
        OnPositionChanged?.Invoke(Position - offset);

        return true;
    }
}