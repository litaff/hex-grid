namespace hex_grid.grid_objects;

using Godot;
using HexGridMap.Vector;
using HexGridObject;
using HexGridObject.Handlers.Rotation;
using HexGridObject.Handlers.Translation;
using HexGridObject.Providers.Position;
using HexGridObject.Providers.Rotation;

[GlobalClass]
public partial class Player : Node3D, ITranslatable, IHexGridObjectHolder, IRotatable
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private float height;
    [Export]
    private float stepHeight;
    [Export]
    private float speed;

    public HexGridObject HexGridObject => GetGridObject();

    private HexGridObject? gridObject;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (HexGridObject.TranslationHandler is IUpdateableTranslationHandler updateable)
        {
            updateable.Update(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Lock input before translation is complete.
        if (HexGridObject.TranslationHandler is IUpdateableTranslationHandler { TranslationComplete: false }) return;
        base._Input(@event);
        OnMovementKeyPressedHandler(@event);
    }

    private void OnMovementKeyPressedHandler(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true } eventKey) return;
        var direction = eventKey.Keycode switch
        {
            Key.Q => CubeHexVector.WestNorth,
            Key.W => CubeHexVector.North,
            Key.E => CubeHexVector.EastNorth,
            Key.A => CubeHexVector.WestSouth,
            Key.S => CubeHexVector.South,
            Key.D => CubeHexVector.EastSouth,
            _ => CubeHexVector.Zero
        };
        if (direction == CubeHexVector.Zero) return;
        
        MoveTowards(direction, eventKey.CtrlPressed);
        
    }
    
    private void MoveTowards(CubeHexVector direction, bool inPlace)
    {
        gridObject?.RotationProvider.RotateTowards(direction);
        if (inPlace) return;
        gridObject?.PositionProvider.Translate(direction);
    }

    void ITranslatable.Translate(Vector3 offset)
    {
        Position += offset;
    }

    public void LookTowards(Vector3 direction)
    {
        LookAt(Position + direction, useModelFront: true);
    }

    private HexGridObject GetGridObject()
    {
        if (gridObject != null) return gridObject;
        var heightData = new HeightData(height, stepHeight);
        var gridPositionProvider = new PositionProvider(new CubeHexVector(awakePosition.X, awakePosition.Y), heightData);
        var rotationProvider = new RotationProvider(CubeHexVector.North);
        var translationProvider = new LinearTranslationHandler(speed, this, heightData);
        var rotationHandler = new InstantRotationHandler(this);
        gridObject = new HexGridObject(gridPositionProvider, rotationProvider, translationProvider, rotationHandler, heightData);
        
        return gridObject;
    }
}