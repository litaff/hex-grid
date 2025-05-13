namespace HexGrid.Entity.Providers.Position;

using System;
using Map.Vector;

public interface IPositionProvider
{
    public HexVector Position { get; }
    /// <summary>
    /// Triggers when position is changed and provides the old position.
    /// </summary>
    public event Action<HexVector> OnPositionChanged;
    /// <summary>
    /// Triggers when the layer is to be changed and provides the relative layer index.
    /// </summary>
    public event Action<int> OnLayerChangeRequested;
    public void Translate(HexVector offset);
    public bool CanTranslate(HexVector offset);
    public void Enable(HexStateProviders providers);
    public void Disable();
}