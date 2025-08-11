namespace HexGrid.Entity.Components.Transform.Position;

using Godot;
using Height;
using Interfaces;
using Map.Vector;

public class InstantPositionHandler : IPositionHandler
{
    private readonly IMovable movable;
    private readonly IHeightManager heightManager;
    private readonly HexVectorConverter converter;

    public bool Enabled { get; private set; }
    public bool MoveComplete { get; private set; }
    
    public event Action? OnPositionReady;

    public InstantPositionHandler(IMovable movable, IHeightManager heightManager, HexVectorConverter converter)
    {
        this.converter = converter;
        this.heightManager = heightManager;
        this.movable = movable;
        MoveComplete = true;
    }

    public void MoveTo(HexVector position)
    {
        if (!Enabled) return;
        MoveComplete = false;
        var offset = converter.ToWorldPosition(position) + heightManager.GetHeight(position) * Vector3.Up - movable.Position;
        movable.Translate(offset);
        OnPositionReady?.Invoke();
        MoveComplete = true;
    }

    public void Update(double deltaTime)
    {
        
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }
}