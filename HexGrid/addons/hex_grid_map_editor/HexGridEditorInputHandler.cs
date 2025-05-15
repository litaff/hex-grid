namespace addons.hex_grid_map_editor;

using System;
using Godot;
using hex_editor;
using HexGrid.Map.Vector;

public class HexGridEditorInputHandler : IHexEditorInputProvider
{
    public bool IsAddHexHeld { get; private set; }
    public bool IsRemoveHexHeld { get; private set; }
    public bool IsDisplayAllLayersHeld { get; private set; }
    public Vector3 CursorPosition { get; private set; }
    public HexVector HexPosition { get; private set; }
    public Vector3 HexCenter { get; private set; }

    public event Action? OnRotateRequested;
    public event Action? OnPipetteRequested;
    public event Action? OnDeselectRequested;
    public event Action? OnDisplayAllLayersRequested;
    public event Action<Vector3>? OnHexCenterUpdated;
    public event Action? OnAddHexRequested;
    public event Action? OnRemoveHexRequest;

    public void UpdateCursorPosition(Vector3 planePosition, Camera3D viewportCamera, InputEventMouseMotion? motion)
    {
        if (motion == null) return;
        var targetPlane = new Plane(Vector3.Up, planePosition);
        var from = viewportCamera.ProjectRayOrigin(motion.Position);
        var to = viewportCamera.ProjectRayNormal(motion.Position);
        var currentCursorPosition = targetPlane.IntersectsRay(from, to);

        if (currentCursorPosition == null) return;
        
        CursorPosition = currentCursorPosition.Value;
        UpdateCurrentHex();
    }

    private void UpdateCurrentHex()
    {
        HexPosition = CursorPosition.ToHexPosition();
        var hexCenter = HexPosition.ToWorldPosition();
        if (hexCenter == HexCenter) return;
        HexCenter = hexCenter;
        OnHexCenterUpdated?.Invoke(HexCenter);
    }

    public EditorPlugin.AfterGuiInput FinalizeInput(Camera3D viewportCamera, InputEvent @event, bool isSelectionActive)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Space }:
                OnDisplayAllLayersRequested?.Invoke();
                IsDisplayAllLayersHeld = true;
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventKey { Pressed: false, Keycode: Key.Space }:
                IsDisplayAllLayersHeld = false;
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventKey { Pressed: true, Keycode: Key.R } when isSelectionActive:
                OnRotateRequested?.Invoke();
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                OnDeselectRequested?.Invoke();
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventKey { Pressed: true, Keycode: Key.Q }:
                OnPipetteRequested?.Invoke();
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } when isSelectionActive:
                OnAddHexRequested?.Invoke();
                IsAddHexHeld = true;
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Left } when isSelectionActive:
                IsAddHexHeld = false;
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right } when isSelectionActive:
                OnRemoveHexRequest?.Invoke();
                IsRemoveHexHeld = true;
                return EditorPlugin.AfterGuiInput.Stop;
            case InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Right } when isSelectionActive:
                IsRemoveHexHeld = false;
                return EditorPlugin.AfterGuiInput.Stop;
            default:
                // Makes it so that input in viewport does not deselect the node, but keeps the camera control behaviour.
                return EditorPlugin.AfterGuiInput.Custom;
        }
    }
}