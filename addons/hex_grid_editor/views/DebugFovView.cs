namespace hex_grid.addons.hex_grid_editor.views;

using System;
using Godot;

[Tool]
public partial class DebugFovView : Control
{
    [Export]
    private CheckButton enableFovButton;
    [Export]
    private SpinBox fovRadius;

    public event Action<int> OnFovDisplayRequested;

    public void Initialize(bool enabled, int previousRadius)
    {
        fovRadius.Value = previousRadius > 0 ? previousRadius : fovRadius.Value;
        enableFovButton.ButtonPressed = enabled;
        enableFovButton.Pressed += OnEnableFovButtonPressedHandler;
        fovRadius.ValueChanged += OnFovRadiusValueChangedHandler;
        OnEnableFovButtonPressedHandler();
    }

    public new void Dispose()
    {
        fovRadius.ValueChanged -= OnFovRadiusValueChangedHandler;
        enableFovButton.Pressed -= OnEnableFovButtonPressedHandler;
        DisableFovRadiusBox();
    }

    private void OnEnableFovButtonPressedHandler()
    {
        if(enableFovButton.ButtonPressed)
        {
            EnableFovRadiusBox();
        }
        else
        {
            DisableFovRadiusBox();
        }
    }

    private void OnFovRadiusValueChangedHandler(double value)
    {
        OnFovDisplayRequested?.Invoke((int)value);
    }

    private void EnableFovRadiusBox()
    {
        fovRadius.GetParent<Control>().Visible = true;
        OnFovDisplayRequested?.Invoke((int)fovRadius.Value);
    }

    private void DisableFovRadiusBox()
    {
        fovRadius.GetParent<Control>().Visible = false;
        // Will be interpreted as "disable"
        OnFovDisplayRequested?.Invoke(0);
    }
}