namespace HexGrid.Entity.Components.Transform.Position;

using Godot;
using Height;
using Interfaces;
using Map.Vector;

public class LinearPositionHandler : IPositionHandler
{
    private readonly IMovable movable;
    private readonly float translationSpeed;
    private readonly IHeightManager heightManager;

    private Vector3 targetPosition;
    private Vector3 translationDirection;

    public bool MoveComplete => movable.Position.IsEqualApprox(targetPosition);
    public bool Enabled { get; private set; }

    public event Action? OnPositionReady;

    public LinearPositionHandler(IMovable movable, IHeightManager heightManager, float translationSpeed)
    {
        this.movable = movable;
        this.heightManager = heightManager;
        this.translationSpeed = translationSpeed;
        Enabled = false;
        
        targetPosition = movable.Position;
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    public void MoveTo(HexVector position)
    {
        if (!Enabled || !MoveComplete) return;
        targetPosition = position.ToWorldPosition() + heightManager.GetHeight(position) * Vector3.Up;
        translationDirection = (targetPosition - movable.Position).Normalized();
        OnPositionReady?.Invoke();
    }

    public void Update(double delta)
    {
        if (!Enabled || MoveComplete) return;

        var step = translationDirection * translationSpeed * (float)delta;
        
        if (EndMovement(step)) return;
        
        movable.Translate(step);
    }

    private bool EndMovement(Vector3 step)
    {
        var offsetLeft = targetPosition - movable.Position;
        if (step.LengthSquared() < offsetLeft.LengthSquared()) return false;
        movable.Translate(offsetLeft);
        return true;
    }
}