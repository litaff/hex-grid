namespace hex_grid.addons.hex_grid_editor.views;

using System;
using Godot;

[Tool]
public partial class ChunkDisplayView : Control
{
    [Export]
    private CheckButton displayChunkButton;
    
    public event Action<bool> OnDisplayChunkRequested;
    
    public void Initialize(bool enabled)
    {
        displayChunkButton.Pressed += OnDisplayChunkButtonPressedHandler;
        OnDisplayChunkButtonPressedHandler();
    }

    private void OnDisplayChunkButtonPressedHandler()
    {
        OnDisplayChunkRequested?.Invoke(displayChunkButton.ButtonPressed);
    }

    public new void Dispose()
    {
        displayChunkButton.Pressed -= OnDisplayChunkButtonPressedHandler;
    }
}