namespace hex_grid.scripts.hex_grid.grid_object.providers.position;

using System;
using vector;

public interface IGridPositionProvider
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
    public void Enable(HexStateProviders providers);
    public void Disable();
}