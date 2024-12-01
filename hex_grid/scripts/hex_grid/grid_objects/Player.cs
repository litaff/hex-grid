namespace hex_grid.grid_objects;

using global::hex_grid_map.vector;
using Godot;
using grid_object;
using grid_object.providers.position;
using grid_object.providers.translation;
using grid_object.providers.translation.providers;

[Tool]
[GlobalClass]
public partial class Player : Node3D, ITranslatable, IGridObjectHolder
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private float height;
    [Export]
    private float stepHeight;
    [Export]
    private float speed;

    public GridObject GridObject => GetGridObject();

    private GridObject? gridObject;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (GridObject.TranslationProvider is IUpdateableTranslationProvider updateable)
        {
            updateable.Update(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Lock input before translation is complete.
        if (GridObject.TranslationProvider is IUpdateableTranslationProvider { TranslationComplete: false }) return;
        
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Q, Echo: false }:
                gridObject?.GridPositionProvider.Translate(CubeHexVector.WestNorth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.W, Echo: false}:
                gridObject?.GridPositionProvider.Translate(CubeHexVector.North);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E, Echo: false}:
                gridObject?.GridPositionProvider.Translate(CubeHexVector.EastNorth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.A, Echo: false}:
                gridObject?.GridPositionProvider.Translate(CubeHexVector.WestSouth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.S, Echo: false}:
                gridObject?.GridPositionProvider.Translate(CubeHexVector.South);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.D, Echo: false}:
                gridObject?.GridPositionProvider.Translate(CubeHexVector.EastSouth);
                break;
        }
        base._Input(@event);
    }

    private GridObject GetGridObject()
    {
        if (gridObject != null) return gridObject;
        var heightData = new HeightData(height, stepHeight);
        var gridPositionProvider = new GridPositionProvider(new CubeHexVector(awakePosition.X, awakePosition.Y), heightData);
        var translationProvider = new LinearTranslationProvider(speed, this, heightData);
        gridObject = new GridObject(gridPositionProvider, translationProvider, heightData);
        
        return gridObject;
    }
}