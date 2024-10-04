namespace hex_grid.addons.hex_grid_editor;

using System;
using Godot;
using scripts;

public class InputHandler
{
    private readonly HexGridMap hexGridMap;
    public Vector3 CursorPosition { get; private set; }
    public CubeHexVector HexPosition { get; private set; }
    public Vector3 HexCenter { get; private set; }
    
    public event Action OnDeselectRequested;
    public event Action<Vector3> OnHexCenterUpdated;
    public event Action OnLeftMouseButtonPressed;
    public event Action OnRightMouseButtonPressed;
    
    public InputHandler(HexGridMap hexGridMap)
    {
        this.hexGridMap = hexGridMap;
    }

    public void UpdateCursorPosition(Camera3D viewportCamera, InputEventMouseMotion motion)
    {
        if (motion == null) return;
        var targetPlane = new Plane(Vector3.Up, hexGridMap.Position);
        var from = viewportCamera.ProjectRayOrigin(motion.Position);
        var to = viewportCamera.ProjectRayNormal(motion.Position);
        var currentCursorPosition = targetPlane.IntersectsRay(from, to);

        if (currentCursorPosition == null) return;
        
        CursorPosition = currentCursorPosition.Value;
        UpdateCurrentHex();
    }

    private void UpdateCurrentHex()
    {
        HexPosition = hexGridMap.GetHexPosition(CursorPosition);
        var hexCenter = hexGridMap.GetWorldPosition(HexPosition);
        if (hexCenter == HexCenter) return;
        HexCenter = hexCenter;
        OnHexCenterUpdated?.Invoke(HexCenter);
    }

    public EditorPlugin.AfterGuiInput FinalizeInput(Camera3D viewportCamera, InputEvent @event, bool isSelectionActive)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                OnDeselectRequested?.Invoke();
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }:
                OnLeftMouseButtonPressed?.Invoke();
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right } when isSelectionActive:
                OnRightMouseButtonPressed?.Invoke();
                return EditorPlugin.AfterGuiInput.Stop;
            default:
                // Makes it so that input in viewport does not deselect the node, but keeps the camera control behaviour.
                return EditorPlugin.AfterGuiInput.Custom;
        }
    }
}