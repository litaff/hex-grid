namespace HexGrid.Entity.Components.Transform.Position.Interfaces;

using Godot;

public interface IMovable
{
    public Vector3 Position { get; }
    public void Translate(Vector3 offset);
}