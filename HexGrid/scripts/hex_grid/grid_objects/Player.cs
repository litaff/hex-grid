namespace hex_grid.grid_objects;

using HexGrid.Entity;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Handlers.Translation;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using HexGrid.Map.Vector;
using Godot;
using HexGrid.Entity.Providers.Block;

[GlobalClass]
public partial class Player : Node3D, ITranslatable, IEntityProvider, IRotatable
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private float height;
    [Export]
    private float stepHeight;
    [Export]
    private float speed;
    [Export]
    private Direction initialForward;

    public Entity Entity => GetEntity();

    private Entity? entity;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Entity.TranslationHandler is IUpdateableTranslationHandler updateable)
        {
            updateable.Update(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Lock input before translation is complete.
        if (Entity.TranslationHandler is IUpdateableTranslationHandler { TranslationComplete: false }) return;
        base._Input(@event);
        OnMovementKeyPressedHandler(@event);
    }

    private void OnMovementKeyPressedHandler(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true } eventKey) return;
        var direction = eventKey.Keycode switch
        {
            Key.Q => HexVector.WestNorth,
            Key.W => HexVector.North,
            Key.E => HexVector.EastNorth,
            Key.A => HexVector.WestSouth,
            Key.S => HexVector.South,
            Key.D => HexVector.EastSouth,
            _ => HexVector.Zero
        };
        if (direction == HexVector.Zero) return;
        
        MoveTowards(direction, eventKey.CtrlPressed);
        
    }
    
    private void MoveTowards(HexVector direction, bool inPlace)
    {
        entity?.RotationProvider.RotateTowards(direction);
        if (inPlace) return;
        entity?.PositionProvider.Translate(direction);
    }

    void ITranslatable.Translate(Vector3 offset)
    {
        Position += offset;
    }

    public void LookTowards(Vector3 direction)
    {
        LookAt(Position + direction, useModelFront: true);
    }

    private Entity GetEntity()
    {
        if (entity != null) return entity;
        
        var heightData = new HeightData(height, stepHeight);
        var positionProvider = new PositionProvider(new HexVector(awakePosition.X, awakePosition.Y), heightData);
        var rotationProvider = new RotationProvider(HexVector.Directions[initialForward]);
        var translationHandler = new LinearTranslationHandler(speed, this, heightData);
        var rotationHandler = new InstantRotationHandler(this);
        var blockProvider = new BlockProvider();
        entity = new Entity(positionProvider, rotationProvider, translationHandler, rotationHandler, blockProvider, heightData);
        
        return entity;
    }
}