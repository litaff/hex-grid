namespace hex_grid.grid_objects;

using global::hex_grid_map.vector;
using Godot;
using hex_grid_object;
using hex_grid_object.providers.position;
using hex_grid_object.providers.translation;
using hex_grid_object.providers.translation.providers;

[Tool]
[GlobalClass]
public partial class Player : Node3D, ITranslatable, IHexGridObjectHolder
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
        if (HexGridObject.TranslationProvider is IUpdateableTranslationProvider updateable)
        {
            updateable.Update(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Lock input before translation is complete.
        if (HexGridObject.TranslationProvider is IUpdateableTranslationProvider { TranslationComplete: false }) return;
        
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Q, Echo: false }:
                gridObject?.HexGridPositionProvider.Translate(CubeHexVector.WestNorth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.W, Echo: false}:
                gridObject?.HexGridPositionProvider.Translate(CubeHexVector.North);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E, Echo: false}:
                gridObject?.HexGridPositionProvider.Translate(CubeHexVector.EastNorth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.A, Echo: false}:
                gridObject?.HexGridPositionProvider.Translate(CubeHexVector.WestSouth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.S, Echo: false}:
                gridObject?.HexGridPositionProvider.Translate(CubeHexVector.South);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.D, Echo: false}:
                gridObject?.HexGridPositionProvider.Translate(CubeHexVector.EastSouth);
                break;
        }
        base._Input(@event);
    }

    private HexGridObject GetGridObject()
    {
        if (gridObject != null) return gridObject;
        var heightData = new HeightData(height, stepHeight);
        var gridPositionProvider = new HexGridPositionProvider(new CubeHexVector(awakePosition.X, awakePosition.Y), heightData);
        var translationProvider = new LinearTranslationProvider(speed, this, heightData);
        gridObject = new HexGridObject(gridPositionProvider, translationProvider, heightData);
        
        return gridObject;
    }
}