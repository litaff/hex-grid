namespace HexGrid.Entity.Tests;

using HexGrid.Entity.Components;

public class MockComponent : Component
{
    public event Action? EnableCalled;
    public event Action? DisableCalled;
    
    public MockComponent(Entity entity) : base(entity) { }

    public override void Enable()
    {
        base.Enable();
        EnableCalled?.Invoke();
    }

    public override void Disable()
    {
        base.Disable();
        DisableCalled?.Invoke();
    }
}