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
    public void Translate(HexVector offset);
    public bool CanTranslate(HexVector offset, out HexVector closest);
    public void Enable(IHexStateProvider hexStateProvider);
    public void Disable();
}