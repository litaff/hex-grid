namespace HexGrid.Entity.Components.Transform.Rotation;

using Map.Vector;

public class RotationProvider
{
    private IRotationHandler? handler;
    private HexVector targetForward;
    
    public bool Enabled { get; private set; }
    public HexVector Forward { get; private set; }
    public HexVector ForRight => new (-Forward.R, -Forward.S);
    public HexVector ForLeft => new (-Forward.S, -Forward.Q);
    public HexVector Backward => -Forward;
    public HexVector BackRight => -ForLeft;
    public HexVector BackLeft => -ForRight;

    public event Action? OnRotationChanged;
    
    public RotationProvider(HexVector initialForward)
    {
        Forward = initialForward.Normalized();
        Enabled = false;
    }

    public void SetHandler(IRotationHandler handler)
    {
        this.handler = handler;
    }
    
    public void RotateTowards(HexVector target)
    {
        if (!Enabled) return;
        var normalizedTarget = target.Normalized();
        if (normalizedTarget == Forward) return;
        
        targetForward = normalizedTarget;

        if (handler is null)
        {
            OnRotationHandledHandler();
            return;
        }
        
        handler.OnRotationHandled += OnRotationHandledHandler;
        handler.RotateTowards(targetForward);
    }

    public void Update(double deltaTime)
    {
        if (!Enabled) return;
        handler?.Update(deltaTime);
    }

    public void Enable()
    {
        Enabled = true;
        OnRotationChanged?.Invoke();
    }

    public void Disable()
    {
        Enabled = false;
    }

    private void OnRotationHandledHandler()
    {
        if (handler is not null)
        {
            handler.OnRotationHandled -= OnRotationHandledHandler;
        }
        Forward = targetForward;
        OnRotationChanged?.Invoke();
    }
}