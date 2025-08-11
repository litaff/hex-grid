namespace hex_grid.grid_objects;

using HexGrid.Entity;
using HexGrid.Map.Vector;
using Godot;
using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Components.Transform;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Position.Interfaces;
using HexGrid.Entity.Components.Transform.Rotation;
using HexGrid.Entity.Managers;

[GlobalClass]
public partial class Player : Node3D, IMovable, IEntityHolder, IRotatable
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private int height;
    [Export]
    private int stepRange;
    [Export]
    private float speed;
    [Export]
    private Direction initialForward;

    private Entity? entity;
    private Transform? transform;
    
    public HexVector InitialPosition => new(awakePosition.X, awakePosition.Y);
    public HexVector InitialRotation => HexVector.Directions[Direction.Forward];
    public IFactoryKey FactoryKey => new PlayerFactoryKey();

    public override void _Process(double delta)
    {
        base._Process(delta);
        transform?.Update(delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (entity is null || transform is null) return;
        
        // Lock input before translation is complete.
        if (transform.Position.IsMoving) return;
        base._Input(@event);
        OnMovementKeyPressedHandler(@event);
    }

    private void OnMovementKeyPressedHandler(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true } eventKey) return;
        var direction = eventKey.Keycode switch
        {
            Key.Q => HexVector.ForLeft,
            Key.W => HexVector.Forward,
            Key.E => HexVector.ForRight,
            Key.A => HexVector.BackLeft,
            Key.S => HexVector.Backward,
            Key.D => HexVector.BackRight,
            _ => HexVector.Zero
        };
        if (direction == HexVector.Zero) return;
        
        MoveTowards(direction, eventKey.CtrlPressed);
        
    }

    private void MoveTowards(HexVector direction, bool inPlace)
    {
        if (entity is null || transform is null) return;
        transform.Rotation.RotateTowards(direction);
        if (inPlace) return;
        transform.Position.Translate(direction);
    }

    void IMovable.Translate(Vector3 offset)
    {
        Position += offset;
    }

    public void LookTowards(Vector3 direction)
    {
        LookAt(Position + direction, useModelFront: true);
    }

    public EntityBlueprint Get()
    {
        var blueprint = new EntityBlueprint();
        blueprint.Add(new TransformDescriptor(stepRange, InitialPosition, InitialRotation));
        blueprint.Add(new HeightDescriptor(height));
        blueprint.Add(new PositionHandlerDescriptor(this, speed));
        blueprint.Add(new RotationHandlerDescriptor(this));
        return blueprint;
    }

    public void Set(Entity entity)
    {
        this.entity = entity;
        transform = entity.GetComponent<Transform>();
        entity.Enable();
    }
}