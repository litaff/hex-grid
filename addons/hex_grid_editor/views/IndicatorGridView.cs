namespace hex_grid.addons.hex_grid_editor.views;

using System;
using Godot;

[Tool]
public partial class IndicatorGridView : Control
{
    [Export]
    private SpinBox indicatorRadius;

    public event Action<int> OnIndicatorSizeChanged;

    public void Initialize(int previousRadius)
    {
        indicatorRadius.Value = previousRadius > 0 ? previousRadius : indicatorRadius.Value;
        OnIndicatorRadiusValueChangedHandler(indicatorRadius.Value);
        indicatorRadius.ValueChanged += OnIndicatorRadiusValueChangedHandler;
    }

    public new void Dispose()
    {
        indicatorRadius.ValueChanged -= OnIndicatorRadiusValueChangedHandler;
    }

    private void OnIndicatorRadiusValueChangedHandler(double value)
    {
        OnIndicatorSizeChanged?.Invoke((int)value);
    }
}