namespace addons.hex_grid_map_editor;

using System;
using Godot;

public interface IHexGridMapPropertyHandler
{
    public Vector3 Position { get; }
    public MeshLibrary MeshLibrary { get; }
    public World3D World3D { get; }
    
    public event Action? OnPropertyChange;
}