namespace addons.hex_grid_editor;

using System;
using Godot;
using Godot.Collections;
using hex_grid_map;

public interface IHexGridMapPropertyHandler
{
    public Vector3 Position { get; }
    public Dictionary<HexType, MeshLibrary> MeshLibraries { get; }
    public World3D World3D { get; }
    
    public event Action? OnPropertyChange;
}