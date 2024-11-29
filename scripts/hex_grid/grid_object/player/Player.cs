namespace hex_grid.scripts.hex_grid.grid_object.player;

using Godot;
using managers;
using providers;
using providers.translation.providers;
using vector;

#if TOOLS
[Tool]
#endif
[GlobalClass]
public partial class Player : GridObject
{
    [Export]
    private float translationSpeed = 1.0f;
    
    public override void _Input(InputEvent @event)
    {
        // Lock input before translation is complete.
        if (TranslationProvider is IUpdateableTranslationProvider { TranslationComplete: false }) return;
        
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Q, Echo: false }:
                GridPositionProvider.Translate(CubeHexVector.WestNorth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.W, Echo: false}:
                GridPositionProvider.Translate(CubeHexVector.North);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E, Echo: false}:
                GridPositionProvider.Translate(CubeHexVector.EastNorth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.A, Echo: false}:
                GridPositionProvider.Translate(CubeHexVector.WestSouth);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.S, Echo: false}:
                GridPositionProvider.Translate(CubeHexVector.South);
                break;
            case InputEventKey { Pressed: true, Keycode: Key.D, Echo: false}:
                GridPositionProvider.Translate(CubeHexVector.EastSouth);
                break;
        }
        base._Input(@event);
    }

    public override void Enable(IGridObjectLayerManager layerManager, HexStateProviders hexStateProviders)
    {
        TranslationProvider ??= new LinearTranslationProvider(translationSpeed, this, HeightData);
        base.Enable(layerManager, hexStateProviders);
    }
}