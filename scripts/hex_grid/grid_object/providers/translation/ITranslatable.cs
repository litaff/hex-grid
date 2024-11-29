namespace hex_grid.scripts.hex_grid.grid_object.providers.translation;

using Godot;

public interface ITranslatable
{
    public Vector3 Position { get; }
    public void Translate(Vector3 offset);
}