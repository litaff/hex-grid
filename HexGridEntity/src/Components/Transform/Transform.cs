namespace HexGrid.Entity.Components.Transform;

using Blocker;
using Managers;
using Map.Vector;
using Position;
using Rotation;

public class Transform : Component
{
    public PositionProvider Position { get; }
    public RotationProvider Rotation { get; }

    public event Action<Transform>? OnUpdated;

    public Transform(Entity entity, IHexManager hexManager, IBlockerManager blockerManager, 
        int stepRange = 0, HexVector position = new(), HexVector forward = new()) : base(entity)
    {
        Position = new PositionProvider(hexManager, blockerManager, position, stepRange);
        Rotation = new RotationProvider(forward);
    }

    public override void Enable()
    {
        base.Enable();
        Position.OnPositionChanged += OnPositionChangedHandler;
        Position.Enable();
        Rotation.Enable();
    }

    public override void Disable()
    {
        Rotation.Disable();
        Position.Disable();
        Position.OnPositionChanged -= OnPositionChangedHandler;
        base.Disable();
    }

    public void Update(double deltaTime)
    {
        if (!Enabled) return;
        Position.Update(deltaTime);
        Rotation.Update(deltaTime);
    }

    private void OnPositionChangedHandler()
    {
        OnUpdated?.Invoke(this);
    }
}