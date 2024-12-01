namespace addons.hex_grid_editor.hex_editor;

using System;
using provider;

public interface IHexEditorInputProvider : IEditorHexPositionProvider
{
    public bool IsAddHexHeld { get; }
    public bool IsRemoveHexHeld { get; }
    public bool IsDisplayAllLayersHeld { get; }
    
    public event Action OnAddHexRequested;
    public event Action OnRemoveHexRequest;
    public event Action OnRotateRequested;
    public event Action OnDisplayAllLayersRequested;
    public event Action OnPipetteRequested;
    public event Action OnDeselectRequested;
}