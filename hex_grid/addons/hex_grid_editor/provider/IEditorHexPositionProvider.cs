namespace addons.hex_grid_editor.provider;

using System;
using Godot;
using HexGridMap.Vector;

public interface IEditorHexPositionProvider
{
    public CubeHexVector HexPosition { get; }
    public Vector3 HexCenter { get; }
    
    public event Action<Vector3> OnHexCenterUpdated;
}