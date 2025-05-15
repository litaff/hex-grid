namespace addons.hex_grid_map_editor.provider;

using System;
using Godot;
using HexGrid.Map.Vector;

public interface IEditorHexPositionProvider
{
    public HexVector HexPosition { get; }
    public Vector3 HexCenter { get; }
    
    public event Action<Vector3> OnHexCenterUpdated;
}