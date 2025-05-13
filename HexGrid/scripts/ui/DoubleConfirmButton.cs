namespace ui;

using System;
using Godot;

[GlobalClass]
public partial class DoubleConfirmButton : Button
{
    [Export]
    private string confirmText = "Are you sure?";
    [Export]
    private float confirmDuration = 1.5f;
    
    private string originalText = "";
    private bool isConfirming;
    
    public event Action? OnConfirmed;

    public override void _Pressed()
    {
        if (isConfirming)
        {
            OnConfirmed?.Invoke();
            ResetButton();
            return;
        }

        isConfirming = true;
        originalText = Text;
        Text = confirmText;
        GetTree().CreateTimer(confirmDuration).Timeout += ResetButton;
    }
    
    private void ResetButton()
    {
        isConfirming = false;
        Text = originalText;
    }
}