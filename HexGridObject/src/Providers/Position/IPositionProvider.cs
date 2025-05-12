namespace HexGridObject.Providers.Position;

using System;
using HexGridMap.Vector;

public interface IPositionProvider
{
    public CubeHexVector Position { get; }
    /// <summary>
    /// Triggers when position is changed and provides the old position.
    /// </summary>
    public event Action<CubeHexVector> OnPositionChanged;
    /// <summary>
    /// Triggers when the layer is to be changed and provides the relative layer index.
    /// </summary>
    public event Action<int> OnLayerChangeRequested;
    public void Translate(CubeHexVector offset);
    public bool CanTranslate(CubeHexVector offset);
    public void Enable(HexStateProviders providers);
    public void Disable();
}