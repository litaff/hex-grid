namespace hex_grid_object.providers.translation;

using Godot;

public interface ITranslatable
{
    public Vector3 Position { get; }
    public void Translate(Vector3 offset);
}